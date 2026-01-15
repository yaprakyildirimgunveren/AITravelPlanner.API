using System.Text.Json;
using AITravelPlanner.Services.Messaging;
using AITravelPlanner.Services.Models;
using AITravelPlanner.Services.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AITravelPlanner.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IMessagePublisher _messagePublisher;

        public RecommendationController(
            IRecommendationService recommendationService,
            IMessagePublisher messagePublisher)
        {
            _recommendationService = recommendationService;
            _messagePublisher = messagePublisher;
        }

        [HttpPost]
        public async Task<IActionResult> GetRecommendations([FromBody] RecommendationRequest request)
        {
            var result = await _recommendationService.GetRecommendationsAsync(request);

            var message = JsonSerializer.Serialize(new
            {
                Type = "recommendation.requested",
                Request = request,
                RequestedAt = DateTimeOffset.UtcNow
            });
            await _messagePublisher.PublishAsync(message);

            return Ok(result);
        }
    }
}
