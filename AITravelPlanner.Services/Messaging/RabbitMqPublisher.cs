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

        public async Task PublishAsync(string message, string? queueName = null, CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("RabbitMQ publishing disabled.");
                return;
            }

            var targetQueue = string.IsNullOrWhiteSpace(queueName) ? _options.QueueName : queueName;
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost
            };

            try
            {
                await using var connection = await factory.CreateConnectionAsync(cancellationToken);
                await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
                await channel.QueueDeclareAsync(
                    queue: targetQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: cancellationToken);

            var body = Encoding.UTF8.GetBytes(message);
            var properties = new BasicProperties { Persistent = true };

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: targetQueue,
                    mandatory: false,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);
                _logger.LogInformation("Message published to queue {QueueName}", targetQueue);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RabbitMQ publish failed.");
            }
        }
    }
}