namespace AITravelPlanner.Services.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string message, string? queueName = null, CancellationToken cancellationToken = default);
    }
}
