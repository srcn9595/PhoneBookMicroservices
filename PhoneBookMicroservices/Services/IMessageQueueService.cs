namespace PhoneBookMicroservices.Services
{
    public interface IMessageQueueService
    {
        void SendMessageToQueue(string queueName, string message);
        string ReceiveMessageFromQueue(string queueName);
    }
}
