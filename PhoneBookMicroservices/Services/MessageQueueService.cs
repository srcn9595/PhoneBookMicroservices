namespace PhoneBookMicroservices.Services
{
    public class MessageQueueService : IMessageQueueService
    {
        private readonly RabbitMqService _rabbitMqService;

        public MessageQueueService(RabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        public void SendMessageToQueue(string queueName, string message)
        {
            _rabbitMqService.SendMessage(queueName, message);
        }

        public string ReceiveMessageFromQueue(string queueName)
        {
            return _rabbitMqService.ReceiveMessage(queueName);
        }
    }
}
