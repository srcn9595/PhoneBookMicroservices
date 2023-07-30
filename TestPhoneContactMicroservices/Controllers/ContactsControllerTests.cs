using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PhoneBookMicroservices.Controllers;
using PhoneBookMicroservices.Models;
using PhoneBookMicroservices;
using System.Collections.Generic;
using PhoneBookMicroservices.Services;

public class ContactsControllerTests
{
    [Fact]
    public async Task GetContacts_ReturnsCorrectNumberOfContacts()
    {
        // Arrange
        var mockContext = new Mock<IContactDirectoryContext>();
        var mockMessageQueueService = new Mock<IMessageQueueService>(); // Add this line

        var expectedContacts = new List<Contact>
        {
            new Contact { Id = Guid.NewGuid(), Name = "Test1", Surname = "Test1", Company = "Test1" },
            new Contact { Id = Guid.NewGuid(), Name = "Test2", Surname = "Test2", Company = "Test2" }
        };

        var mockSet = new Mock<DbSet<Contact>>();
        mockSet.As<IQueryable<Contact>>().Setup(m => m.Provider).Returns(expectedContacts.AsQueryable().Provider);
        mockSet.As<IQueryable<Contact>>().Setup(m => m.Expression).Returns(expectedContacts.AsQueryable().Expression);
        mockSet.As<IQueryable<Contact>>().Setup(m => m.ElementType).Returns(expectedContacts.AsQueryable().ElementType);
        mockSet.As<IQueryable<Contact>>().Setup(m => m.GetEnumerator()).Returns(expectedContacts.AsQueryable().GetEnumerator());

        // Add this line
        mockSet.As<IAsyncEnumerable<Contact>>().Setup(m => m.GetAsyncEnumerator(new CancellationToken()))
         .Returns(new TestAsyncEnumerator<Contact>(expectedContacts.GetEnumerator()));

        mockContext.Setup(ctx => ctx.Contacts).Returns(mockSet.Object);

        var controller = new ContactsController(mockContext.Object, mockMessageQueueService.Object); // Update this line

        // Act
        var result = await controller.GetContacts();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Contact>>>(result);
        var contacts = Assert.IsType<List<Contact>>(actionResult.Value);
        Assert.Equal(expectedContacts.Count, contacts.Count);
    }
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask(Task.CompletedTask);
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }
    }

    public class TestAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerable<T> _inner;

        public TestAsyncEnumerable(IEnumerable<T> inner)
        {
            _inner = inner;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            return new TestAsyncEnumerator<T>(_inner.GetEnumerator());
        }
    }
    [Fact]
    public async Task CreateContact_CreatesContact()
    {
        // Arrange
        var mockContext = new Mock<IContactDirectoryContext>();
        var mockMessageQueueService = new Mock<IMessageQueueService>();

        var newContact = new Contact { Name = "Test2", Surname = "Test2", Company = "Test2" };

        mockContext.Setup(ctx => ctx.Contacts.Add(newContact));
        mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

        var controller = new ContactsController(mockContext.Object, mockMessageQueueService.Object);

        // Act
        var result = await controller.CreateContact(newContact);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Contact>>(result);
        var contact = Assert.IsType<Contact>(actionResult.Value);
        Assert.Equal(newContact.Name, contact.Name);
        Assert.Equal(newContact.Surname, contact.Surname);
        Assert.Equal(newContact.Company, contact.Company);
    }
    [Fact]
    public async Task UpdateContact_UpdatesContact()
    {
        // Arrange
        var mockContext = new Mock<IContactDirectoryContext>();
        var mockMessageQueueService = new Mock<IMessageQueueService>();

        var existingContact = new Contact { Id = Guid.NewGuid(), Name = "Test", Surname = "Test", Company = "Test" };
        var updatedContact = new Contact { Id = existingContact.Id, Name = "Updated", Surname = "Updated", Company = "Updated" };

        mockContext.Setup(ctx => ctx.Contacts.FindAsync(existingContact.Id)).ReturnsAsync(existingContact);
        mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

        var controller = new ContactsController(mockContext.Object, mockMessageQueueService.Object);

        // Act
        var result = await controller.UpdateContact(existingContact.Id, updatedContact);

        // Assert
        var actionResult = Assert.IsType<NoContentResult>(result);
        mockContext.Verify(ctx => ctx.Update(updatedContact), Times.Once);
        mockMessageQueueService.Verify(mqs => mqs.SendMessageToQueue("ContactUpdated", It.IsAny<string>()), Times.Once);
    }
    [Fact]
    public async Task DeleteContact_DeletesContact()
    {
        // Arrange
        var mockContext = new Mock<IContactDirectoryContext>();
        var mockMessageQueueService = new Mock<IMessageQueueService>();

        var existingContact = new Contact { Id = Guid.NewGuid(), Name = "Test", Surname = "Test", Company = "Test" };

        mockContext.Setup(ctx => ctx.Contacts.FindAsync(existingContact.Id)).ReturnsAsync(existingContact);
        mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

        var controller = new ContactsController(mockContext.Object, mockMessageQueueService.Object);

        // Act
        var result = await controller.DeleteContact(existingContact.Id);

        // Assert
        var actionResult = Assert.IsType<NoContentResult>(result);
        mockContext.Verify(ctx => ctx.Contacts.Remove(existingContact), Times.Once);
        mockMessageQueueService.Verify(mqs => mqs.SendMessageToQueue("ContactDeleted", It.IsAny<string>()), Times.Once);
    }
}


