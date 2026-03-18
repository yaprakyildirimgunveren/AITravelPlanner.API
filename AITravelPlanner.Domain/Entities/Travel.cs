namespace AITravelPlanner.Domain.Entities
{
    public enum TravelType { Business, Leisure, Adventure, Family, Solo }

    public class Travel : BaseEntity
    {
        public string Destination { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TravelType Type { get; set; }
        public decimal Budget { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
