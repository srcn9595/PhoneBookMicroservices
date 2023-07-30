using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPhoneContactMicroservices.Services
{
    public interface IMessageQueueServiceTests
    {
        void SendMessageToQueue(string queueName, string message);
        string ReceiveMessageFromQueue(string queueName);
    }
}
