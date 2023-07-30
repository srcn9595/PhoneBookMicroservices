using Microsoft.AspNetCore.Mvc;
using Moq;
using PhoneBookMicroservices.Controllers;
using PhoneBookMicroservices.Models;
using PhoneBookMicroservices.Services;
using PhoneBookMicroservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestPhoneContactMicroservices.Controllers
{
    public class ContactDetailsControllerTests
    {
        [Fact]
        public async Task GetContactDetails_ReturnsContactDetails()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();
            var contactDetails = new List<ContactDetail>
{
    new ContactDetail { ContactId = contactId, InfoContent = "Test1", InfoTypeValue = 1 },
    new ContactDetail { ContactId = contactId, InfoContent = "Test2", InfoTypeValue = 2 }
};

            var mockSet = new MockDbSet<ContactDetail>();
            foreach (var detail in contactDetails)
            {
                mockSet.Add(detail);
            }

            mockContext.Setup(ctx => ctx.ContactDetails).Returns(mockSet);

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.GetContactDetails(contactId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ContactDetail>>>(result);
            var returnedContactDetails = Assert.IsType<List<ContactDetail>>(actionResult.Value);
            Assert.Equal(2, returnedContactDetails.Count);
        }
        [Fact]
        public async Task CreateContactDetail_ValidContactDetail_CreatesContactDetail()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();
            var newContactDetail = new ContactDetail { ContactId = contactId, InfoContent = "Test", InfoTypeValue = 1 };

            mockContext.Setup(ctx => ctx.ContactDetails.Add(newContactDetail));
            mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.CreateContactDetail(contactId, newContactDetail);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ContactDetail>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var contactDetail = Assert.IsType<ContactDetail>(createdAtActionResult.Value);
            Assert.Equal(newContactDetail.InfoContent, contactDetail.InfoContent);
            Assert.Equal(newContactDetail.InfoTypeValue, contactDetail.InfoTypeValue);
        }
        [Fact]
        public async Task GetContactDetails_NoContactDetails_ReturnsNotFound()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();

            var mockSet = new MockDbSet<ContactDetail>();

            mockContext.Setup(ctx => ctx.ContactDetails).Returns(mockSet);

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.GetContactDetails(contactId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task CreateContactDetail_ContactIdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();
            var newContactDetail = new ContactDetail { ContactId = Guid.NewGuid(), InfoContent = "Test", InfoTypeValue = 1 };

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.CreateContactDetail(contactId, newContactDetail);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task CreateContactDetail_CreatesContactDetail()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();
            var newContactDetail = new ContactDetail { ContactId = contactId, InfoContent = "Test", InfoTypeValue = 1 };

            mockContext.Setup(ctx => ctx.ContactDetails.Add(newContactDetail));
            mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.CreateContactDetail(contactId, newContactDetail);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ContactDetail>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var contactDetail = Assert.IsType<ContactDetail>(createdAtActionResult.Value);
            Assert.Equal(newContactDetail.InfoContent, contactDetail.InfoContent);
            Assert.Equal(newContactDetail.InfoTypeValue, contactDetail.InfoTypeValue);
        }








        [Fact]
        public async Task UpdateContactDetail_UpdatesContactDetail()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();
            var contactDetailId = Guid.NewGuid();
            var existingContactDetail = new ContactDetail { Id = contactDetailId, ContactId = contactId, InfoContent = "Test", InfoTypeValue = 1 };
            var updatedContactDetail = new ContactDetail { Id = contactDetailId, ContactId = contactId, InfoContent = "Updated Test", InfoTypeValue = 2 };

            mockContext.Setup(ctx => ctx.ContactDetails.FindAsync(contactDetailId)).ReturnsAsync(existingContactDetail);
            mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.UpdateContactDetail(contactId, contactDetailId, updatedContactDetail);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(updatedContactDetail.InfoContent, existingContactDetail.InfoContent);
            Assert.Equal(updatedContactDetail.InfoTypeValue, existingContactDetail.InfoTypeValue);
        }
        [Fact]
        public async Task UpdateContactDetail_ContactDetailNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();
            var contactDetailId = Guid.NewGuid();
            var updatedContactDetail = new ContactDetail { Id = contactDetailId, ContactId = contactId, InfoContent = "Updated Test", InfoTypeValue = 2 };

            mockContext.Setup(ctx => ctx.ContactDetails.FindAsync(contactDetailId)).ReturnsAsync((ContactDetail)null);

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.UpdateContactDetail(contactId, contactDetailId, updatedContactDetail);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task UpdateContactDetail_ValidContactDetail_UpdatesContactDetail()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();
            var contactDetailId = Guid.NewGuid();
            var existingContactDetail = new ContactDetail { Id = contactDetailId, ContactId = contactId, InfoContent = "Test", InfoTypeValue = 1 };
            var updatedContactDetail = new ContactDetail { Id = contactDetailId, ContactId = contactId, InfoContent = "Updated Test", InfoTypeValue = 2 };

            mockContext.Setup(ctx => ctx.ContactDetails.FindAsync(contactDetailId)).ReturnsAsync(existingContactDetail);
            mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.UpdateContactDetail(contactId, contactDetailId, updatedContactDetail);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(updatedContactDetail.InfoContent, existingContactDetail.InfoContent);
            Assert.Equal(updatedContactDetail.InfoTypeValue, existingContactDetail.InfoTypeValue);
        }

        [Fact]
        public async Task UpdateContactDetail_ContactIdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();
            var contactDetailId = Guid.NewGuid();
            var updatedContactDetail = new ContactDetail { Id = contactDetailId, ContactId = Guid.NewGuid(), InfoContent = "Updated Test", InfoTypeValue = 2 };

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.UpdateContactDetail(contactId, contactDetailId, updatedContactDetail);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }


        [Fact]
        public async Task DeleteContactDetail_DeletesContactDetail()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();
            var contactDetailId = Guid.NewGuid();
            var existingContactDetail = new ContactDetail { Id = contactDetailId, ContactId = contactId, InfoContent = "Test", InfoTypeValue = 1 };

            mockContext.Setup(ctx => ctx.ContactDetails.FindAsync(contactDetailId)).ReturnsAsync(existingContactDetail);
            mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));
            mockContext.Setup(ctx => ctx.ContactDetailExists(contactDetailId)).Returns(true); // Add this line to mock ContactDetailExists method.

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.DeleteContactDetail(contactId, contactDetailId);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            mockContext.Verify(ctx => ctx.ContactDetails.Remove(existingContactDetail), Times.Once);
        }

      
        [Fact]
        public async Task DeleteContactDetail_ContactDetailNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();
            var contactDetailId = Guid.NewGuid();

            mockContext.Setup(ctx => ctx.ContactDetails.FindAsync(contactDetailId)).ReturnsAsync((ContactDetail)null); // Return null to simulate the contact detail not found.
            mockContext.Setup(ctx => ctx.ContactDetailExists(contactDetailId)).Returns(false); // Add this line to mock ContactDetailExists method.

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.DeleteContactDetail(contactId, contactDetailId);

            // Assert
            var actionResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteContactDetail_ContactIdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();
            var contactDetailId = Guid.NewGuid();
            var existingContactDetail = new ContactDetail { Id = contactDetailId, ContactId = Guid.NewGuid(), InfoContent = "Test", InfoTypeValue = 1 };

            mockContext.Setup(ctx => ctx.ContactDetails.FindAsync(contactDetailId)).ReturnsAsync(existingContactDetail);

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.DeleteContactDetail(contactId, contactDetailId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteContactDetail_ValidContactDetail_DeletesContactDetail()
        {
            // Arrange
            var mockContext = new Mock<IContactDirectoryContext>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();

            var contactId = Guid.NewGuid();
            var contactDetailId = Guid.NewGuid();
            var existingContactDetail = new ContactDetail { Id = contactDetailId, ContactId = contactId, InfoContent = "Test", InfoTypeValue = 1 };

            mockContext.Setup(ctx => ctx.ContactDetails.FindAsync(contactDetailId)).ReturnsAsync(existingContactDetail);
            mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            var controller = new ContactDetailsController(mockContext.Object, mockMessageQueueService.Object);

            // Act
            var result = await controller.DeleteContactDetail(contactId, contactDetailId);

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            mockContext.Verify(ctx => ctx.ContactDetails.Remove(existingContactDetail), Times.Once);
        }


    }
}
