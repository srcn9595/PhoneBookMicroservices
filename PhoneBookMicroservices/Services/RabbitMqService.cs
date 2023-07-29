using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace PhoneBookMicroservices.Services
{
    public class RabbitMqService
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqService()
        {
            _factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void SendMessage(string queueName, string message)
        {
            _channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
        }

        public string ReceiveMessage(string queueName)
        {
            var consumer = new EventingBasicConsumer(_channel);
            string messageContent = null;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                messageContent = Encoding.UTF8.GetString(body);
            };

            _channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            return messageContent;
        }
    }
}
