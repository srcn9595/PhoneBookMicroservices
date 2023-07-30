using Microsoft.AspNetCore.Mvc;
using PhoneBookMicroservices.Services;

namespace PhoneBookMicroservices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RabbitMqController:ControllerBase
    {
        private readonly IMessageQueueService _messageQueueService;

        public RabbitMqController(IMessageQueueService messageQueueService)
        {
            _messageQueueService = messageQueueService;
        }
        [HttpGet("send")]
        public IActionResult SendMessage()
        {
            _messageQueueService.SendMessageToQueue("testQueue", "Hello, RabbitMQ!");
            return Ok("Message sent.");
        }
        [HttpGet("receive")]
        public IActionResult ReceiveMessage()
        {
            var message = _messageQueueService.ReceiveMessageFromQueue("testQueue");
            if (string.IsNullOrEmpty(message))
            {
                return NotFound(); 
            }

            return Ok($"Received message: {message}");
        }
    }
}
