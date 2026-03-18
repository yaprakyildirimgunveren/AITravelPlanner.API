namespace AITravelPlanner.Domain.Entities
{
    public class Flight : BaseEntity
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public decimal Price { get; set; }
    }
}
