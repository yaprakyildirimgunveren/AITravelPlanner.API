using AITravelPlanner.Domain.Entities;
using AITravelPlanner.Services.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace AITravelPlanner.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelController : ControllerBase
    {
        private readonly ITravelService _travelService;

        public TravelController(ITravelService travelService)
        {
            _travelService = travelService;
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
