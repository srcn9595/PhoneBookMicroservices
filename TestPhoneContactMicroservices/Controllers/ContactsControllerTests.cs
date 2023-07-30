using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PhoneBookMicroservices.Controllers;
using PhoneBookMicroservices.Models;
using PhoneBookMicroservices;
using System.Collections.Generic;
using PhoneBookMicroservices.Services;
using TestPhoneContactMicroservices;

public class ContactsControllerTests
{
   

    [Fact]
    public async Task CreateContact_InvalidContact_ReturnsBadRequest()
    {
        // Arrange
        var mockContext = new Mock<IContactDirectoryContext>();
        var mockMessageQueueService = new Mock<IMessageQueueService>();

        var controller = new ContactsController(mockContext.Object, mockMessageQueueService.Object);

        // Act
        var result = await controller.CreateContact(null);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task UpdateContact_NonExistingContact_ReturnsNotFound()
    {
        // Arrange
        var mockContext = new Mock<IContactDirectoryContext>();
        var mockMessageQueueService = new Mock<IMessageQueueService>();

        var nonExistingId = Guid.NewGuid();
        var updatedContact = new Contact { Id = nonExistingId, Name = "Updated", Surname = "Updated", Company = "Updated" };

        mockContext.Setup(ctx => ctx.Contacts.FindAsync(nonExistingId)).ReturnsAsync((Contact)null);

        var controller = new ContactsController(mockContext.Object, mockMessageQueueService.Object);

        // Act
        var result = await controller.UpdateContact(nonExistingId, updatedContact);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public void AddContact_AddsContactToContext()
    {
        // Arrange
        var context = new MockContactDirectoryContext();
        var contact = new Contact { Id = Guid.NewGuid(), Name = "Test", Surname = "Test", Company = "Test" };

        // Act
        context.Contacts.Add(contact);

        // Assert
        Assert.Contains(contact, context.Contacts);
    }

    [Fact]
    public async Task DeleteContact_NonExistingContact_ReturnsNotFound()
    {
        // Arrange
        var mockContext = new Mock<IContactDirectoryContext>();
        var mockMessageQueueService = new Mock<IMessageQueueService>();

        var nonExistingId = Guid.NewGuid();

        mockContext.Setup(ctx => ctx.Contacts.FindAsync(nonExistingId)).ReturnsAsync((Contact)null);

        var controller = new ContactsController(mockContext.Object, mockMessageQueueService.Object);

        // Act
        var result = await controller.DeleteContact(nonExistingId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateContact_SaveChangesThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var mockContext = new Mock<IContactDirectoryContext>();
        var mockMessageQueueService = new Mock<IMessageQueueService>();

        var newContact = new Contact { Name = "Test2", Surname = "Test2", Company = "Test2" };

        mockContext.Setup(ctx => ctx.Contacts.Add(newContact));
        mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        var controller = new ContactsController(mockContext.Object, mockMessageQueueService.Object);

        // Act
        var result = await controller.CreateContact(newContact);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task UpdateContact_SaveChangesThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var mockContext = new Mock<IContactDirectoryContext>();
        var mockMessageQueueService = new Mock<IMessageQueueService>();

        var existingContact = new Contact { Id = Guid.NewGuid(), Name = "Test", Surname = "Test", Company = "Test" };
        var updatedContact = new Contact { Id = existingContact.Id, Name = "Updated", Surname = "Updated", Company = "Updated" };

        mockContext.Setup(ctx => ctx.Contacts.FindAsync(existingContact.Id)).ReturnsAsync(existingContact);
        mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        var controller = new ContactsController(mockContext.Object, mockMessageQueueService.Object);

        // Act
        var result = await controller.UpdateContact(existingContact.Id, updatedContact);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }


    [Fact]
    public async Task DeleteContact_SaveChangesThrowsException_ReturnsBadRequest()
    {
        // Arrange
        var mockContext = new Mock<IContactDirectoryContext>();
        var mockMessageQueueService = new Mock<IMessageQueueService>();

        var existingContact = new Contact { Id = Guid.NewGuid(), Name = "Test", Surname = "Test", Company = "Test" };

        mockContext.Setup(ctx => ctx.Contacts.FindAsync(existingContact.Id)).ReturnsAsync(existingContact);
        mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        var controller = new ContactsController(mockContext.Object, mockMessageQueueService.Object);

        // Act
        var result = await controller.DeleteContact(existingContact.Id);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
    [Fact]
    public void RemoveContact_RemovesContactFromContext()
    {
        // Arrange
        var context = new MockContactDirectoryContext();
        var contact = new Contact { Id = Guid.NewGuid(), Name = "Test", Surname = "Test", Company = "Test" };
        context.Contacts.Add(contact);

        // Act
        context.Contacts.Remove(contact);

        // Assert
        Assert.DoesNotContain(contact, context.Contacts);
    }
    [Fact]
    public void Add_AddsItemToSet()
    {
        // Arrange
        var set = new MockDbSet<Contact>();
        var contact = new Contact { Id = Guid.NewGuid(), Name = "Test", Surname = "Test", Company = "Test" };

        // Act
        set.Add(contact);

        // Assert
        Assert.Contains(contact, set);
    }

    [Fact]
    public void Remove_RemovesItemFromSet()
    {
        // Arrange
        var set = new MockDbSet<Contact>();
        var contact = new Contact { Id = Guid.NewGuid(), Name = "Test", Surname = "Test", Company = "Test" };
        set.Add(contact);

        // Act
        set.Remove(contact);

        // Assert
        Assert.DoesNotContain(contact, set);
    }
}



