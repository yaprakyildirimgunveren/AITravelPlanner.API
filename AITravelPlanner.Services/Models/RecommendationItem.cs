namespace AITravelPlanner.Services.Models
{
    public class RecommendationItem
    {
        public string Destination { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }
        public List<string> Activities { get; set; } = new();
    }
}
