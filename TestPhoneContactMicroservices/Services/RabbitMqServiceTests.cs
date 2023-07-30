using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPhoneContactMicroservices.Services
{
    public class RabbitMqServiceTests
    {
        [Fact]
        public void SendMessage_WhenCalledWithQueueName_ShouldSendMessageToQueue()
        {
            // Arrange
            var fakeRabbitMqService = new FakeRabbitMqService();
            var queueName = "testQueue";
            var message = "Test message";

            // Act
            fakeRabbitMqService.SendMessageToQueue(queueName, message);

            // Assert
            var receivedMessage = fakeRabbitMqService.ReceiveMessageFromQueue(queueName);
            Assert.Equal(message, receivedMessage);
        }

        [Fact]
        public void ReceiveMessage_WhenQueueIsEmpty_ShouldReturnNull()
        {
            // Arrange
            var fakeRabbitMqService = new FakeRabbitMqService();
            var queueName = "testQueue";

            // Act
            var receivedMessage = fakeRabbitMqService.ReceiveMessageFromQueue(queueName);

            // Assert
            Assert.Null(receivedMessage);
        }
    }
}
