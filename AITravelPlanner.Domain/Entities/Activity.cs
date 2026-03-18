namespace AITravelPlanner.Domain.Entities
{
    public class Activity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int TravelId { get; set; }
        public Travel Travel { get; set; } = null!;

        public Activity() { }

    }
}
