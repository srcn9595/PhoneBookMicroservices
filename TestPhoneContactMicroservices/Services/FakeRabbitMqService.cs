using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPhoneContactMicroservices.Services
{
    public class FakeRabbitMqService : IMessageQueueServiceTests
    {
        private Dictionary<string, string> _queues;

        public FakeRabbitMqService()
        {
            _queues = new Dictionary<string, string>();
        }
        public void SendMessageToQueue(string queueName, string message)
        {
            if (!_queues.ContainsKey(queueName))
            {
                _queues[queueName] = message;
            }
            else
            {
                _queues[queueName] += $"; {message}";
            }
        }

        public string ReceiveMessageFromQueue(string queueName)
        {
            return _queues.TryGetValue(queueName, out string message) ? message : null;
        }
    }
}
