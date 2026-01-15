using System.Text;
using AITravelPlanner.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AITravelPlanner.Services.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqPublisher> _logger;

        public RabbitMqPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public Task PublishAsync(string message, string? queueName = null, CancellationToken cancellationToken = default)
        {
            var targetQueue = string.IsNullOrWhiteSpace(queueName) ? _options.QueueName : queueName;
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: targetQueue, durable: true, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(message);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: string.Empty, routingKey: targetQueue, basicProperties: properties, body: body);
            _logger.LogInformation("Message published to queue {QueueName}", targetQueue);

            return Task.CompletedTask;
        }
    }
}
