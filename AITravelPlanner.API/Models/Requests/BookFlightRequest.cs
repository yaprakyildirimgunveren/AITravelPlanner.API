namespace AITravelPlanner.API.Models.Requests
{
    public class BookFlightRequest
    {
        public int FlightId { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
    }
}
