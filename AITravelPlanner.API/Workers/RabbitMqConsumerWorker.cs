using System.Text;
using AITravelPlanner.Services.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AITravelPlanner.API.Workers
{
    public class RabbitMqConsumerWorker : BackgroundService
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqConsumerWorker> _logger;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMqConsumerWorker(
            IOptions<RabbitMqOptions> options,
            ILogger<RabbitMqConsumerWorker> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _options.QueueName, durable: true, exclusive: false, autoDelete: false);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
            {
                _logger.LogWarning("RabbitMQ channel not initialized.");
                return Task.CompletedTask;
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (_, eventArgs) =>
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                _logger.LogInformation("RabbitMQ message consumed: {Message}", message);
                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: _options.QueueName, autoAck: false, consumer: consumer);
            return Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            return base.StopAsync(cancellationToken);
        }
    }
}
