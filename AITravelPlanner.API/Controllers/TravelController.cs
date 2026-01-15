using AITravelPlanner.Domain.Entities;
using AITravelPlanner.Services.Messaging;
using AITravelPlanner.Services.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AITravelPlanner.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelController : ControllerBase
    {
        private readonly ITravelService _travelService;
        private readonly IMessagePublisher _messagePublisher;

        public TravelController(ITravelService travelService, IMessagePublisher messagePublisher)
        {
            _travelService = travelService;
            _messagePublisher = messagePublisher;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTravelsByUser(int userId)
        {
            var travels = await _travelService.GetTravelsByUserAsync(userId);
            return Ok(travels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTravelById(int id)
        {
            var travel = await _travelService.GetByIdAsync(id);
            if (travel == null) return NotFound();
            return Ok(travel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTravel([FromBody] Travel travel)
        {
            var created = await _travelService.CreateAsync(travel);
            var message = JsonSerializer.Serialize(new
            {
                Type = "travel.created",
                TravelId = created.Id,
                UserId = created.UserId,
                CreatedAt = DateTimeOffset.UtcNow
            });
            await _messagePublisher.PublishAsync(message);
            return CreatedAtAction(nameof(GetTravelById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTravel(int id, [FromBody] Travel travel)
        {
            if (id != travel.Id) return BadRequest();
            var updated = await _travelService.UpdateAsync(travel);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTravel(int id)
        {
            var deleted = await _travelService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
