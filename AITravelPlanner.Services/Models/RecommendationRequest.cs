namespace AITravelPlanner.Services.Models
{
    public class RecommendationRequest
    {
        public int UserId { get; set; }
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public DateTime TravelDate { get; set; }
        public decimal Budget { get; set; }
        public int Travelers { get; set; } = 1;
    }
}
