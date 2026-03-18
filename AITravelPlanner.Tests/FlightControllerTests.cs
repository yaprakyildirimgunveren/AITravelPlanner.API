using AITravelPlanner.API.Controllers;
using AITravelPlanner.Domain.Entities;
using AITravelPlanner.Services.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AITravelPlanner.Tests
{
    public class FlightControllerTests
    {
        [Fact]
        public async Task SearchFlights_ReturnsOk_WithResults()
        {
            var service = new Mock<IFlightService>();
            service.Setup(s => s.SearchFlightsAsync("IST", "ATH"))
                .ReturnsAsync(new List<Flight>
                {
                    new Flight
                    {
                        Id = 1,
                        From = "IST",
                        To = "ATH",
                        DepartureDate = DateTime.UtcNow.AddDays(5),
                        Price = 250
                    }
                });

            var controller = new FlightController(service.Object);

            var result = await controller.SearchFlights("IST", "ATH");

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
            Assert.Single(list);
        }

        [Fact]
        public async Task GetFlight_ReturnsNotFound_WhenMissing()
        {
            var service = new Mock<IFlightService>();
            service.Setup(s => s.GetFlightByIdAsync(99)).ReturnsAsync((Flight?)null);

            var controller = new FlightController(service.Object);

            var result = await controller.GetFlight(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}