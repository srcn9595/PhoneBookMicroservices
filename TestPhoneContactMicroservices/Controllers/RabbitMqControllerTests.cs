using Microsoft.AspNetCore.Mvc;
using Moq;
using PhoneBookMicroservices.Controllers;
using PhoneBookMicroservices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPhoneContactMicroservices.Controllers
{
    public class RabbitMqControllerTests
    {
        [Fact]
        public void SendMessage_SendsMessageToQueue()
        {
            // Arrange
            var mockMessageQueueService = new Mock<IMessageQueueService>();
            var controller = new RabbitMqController(mockMessageQueueService.Object);

            // Act
            controller.SendMessage();

            // Assert
            mockMessageQueueService.Verify(m => m.SendMessageToQueue("testQueue", "Hello, RabbitMQ!"), Times.Once);
        }
        [Fact]
        public void ReceiveMessage_ReceivesMessageFromQueue()
        {
            // Arrange
            var mockMessageQueueService = new Mock<IMessageQueueService>();
            var controller = new RabbitMqController(mockMessageQueueService.Object);
            var expectedMessage = "Test message";
            mockMessageQueueService.Setup(m => m.ReceiveMessageFromQueue("testQueue")).Returns(expectedMessage);

            // Act
            var result = controller.ReceiveMessage();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var receivedMessage = Assert.IsType<string>(okResult.Value);
            Assert.Equal($"Received message: {expectedMessage}", receivedMessage);
        }
        [Fact]
        public void ReceiveMessage_WithMessage_ReturnsOkResult()
        {
            // Arrange
            var mockMessageQueueService = new Mock<IMessageQueueService>();
            var controller = new RabbitMqController(mockMessageQueueService.Object);
            var expectedMessage = "Test message";
            mockMessageQueueService.Setup(m => m.ReceiveMessageFromQueue("testQueue")).Returns(expectedMessage);

            // Act
            var result = controller.ReceiveMessage();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var receivedMessage = Assert.IsType<string>(okResult.Value);
            Assert.Equal($"Received message: {expectedMessage}", receivedMessage);
        }
        [Fact]
        public void ReceiveMessage_WithoutMessage_ReturnsNotFoundResult()
        {
            // Arrange
            var mockMessageQueueService = new Mock<IMessageQueueService>();
            var controller = new RabbitMqController(mockMessageQueueService.Object);
            string expectedMessage = null; // No message in the queue
            mockMessageQueueService.Setup(m => m.ReceiveMessageFromQueue("testQueue")).Returns(expectedMessage);

            // Act
            var result = controller.ReceiveMessage();

            // Assert
            Assert.IsType<NotFoundResult>(result); // Corrected assertion
        }

    }
}
