namespace AITravelPlanner.Services.Options
{
    public class RabbitMqOptions
    {
        public bool Enabled { get; set; } = true;
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string QueueName { get; set; } = "travel-events";
    }
}
