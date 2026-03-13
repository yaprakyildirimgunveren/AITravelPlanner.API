using AITravelPlanner.API.Controllers;
using AITravelPlanner.Services.Messaging;
using AITravelPlanner.Services.Models;
using AITravelPlanner.Services.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AITravelPlanner.Tests
{
    public class RecommendationControllerTests
    {
        [Fact]
        public async Task GetRecommendations_ReturnsOk_AndPublishesEvent()
        {
            var service = new Mock<IRecommendationService>();
            var publisher = new Mock<IMessagePublisher>();

            service.Setup(s => s.GetRecommendationsAsync(It.IsAny<RecommendationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RecommendationResponse
                {
                    Recommendations = new List<RecommendationItem>
                    {
                        new RecommendationItem
                        {
                            Destination = "Athens",
                            Reason = "Demo",
                            EstimatedCost = 500
                        }
                    }
                });

            var controller = new RecommendationController(service.Object, publisher.Object);

            var request = new RecommendationRequest
            {
                UserId = 1,
                From = "IST",
                To = "ATH",
                TravelDate = DateTime.UtcNow.Date.AddDays(30),
                Budget = 1000,
                Travelers = 1
            };

            var result = await controller.GetRecommendations(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<RecommendationResponse>(okResult.Value);

            publisher.Verify(p => p.PublishAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}