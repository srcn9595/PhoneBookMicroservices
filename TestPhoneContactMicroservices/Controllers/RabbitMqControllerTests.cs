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

    }
}
