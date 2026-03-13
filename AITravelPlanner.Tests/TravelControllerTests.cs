using AITravelPlanner.API.Controllers;
using AITravelPlanner.Domain.Entities;
using AITravelPlanner.Services.Messaging;
using AITravelPlanner.Services.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AITravelPlanner.Tests
{
    public class TravelControllerTests
    {
        [Fact]
        public async Task GetTravelById_ReturnsNotFound_WhenMissing()
        {
            var service = new Mock<ITravelService>();
            var publisher = new Mock<IMessagePublisher>();
            service.Setup(s => s.GetByIdAsync(42)).ReturnsAsync((Travel?)null);

            var controller = new TravelController(service.Object, publisher.Object);

            var result = await controller.GetTravelById(42);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateTravel_ReturnsCreated_AndPublishesEvent()
        {
            var service = new Mock<ITravelService>();
            var publisher = new Mock<IMessagePublisher>();

            var travel = new Travel
            {
                Id = 1,
                Destination = "Athens",
                StartDate = DateTime.UtcNow.Date.AddDays(10),
                EndDate = DateTime.UtcNow.Date.AddDays(12),
                Type = TravelType.Leisure,
                Budget = 900,
                UserId = 5
            };

            service.Setup(s => s.CreateAsync(It.IsAny<Travel>())).ReturnsAsync(travel);

            var controller = new TravelController(service.Object, publisher.Object);

            var result = await controller.CreateTravel(travel);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(TravelController.GetTravelById), created.ActionName);
            publisher.Verify(p => p.PublishAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}