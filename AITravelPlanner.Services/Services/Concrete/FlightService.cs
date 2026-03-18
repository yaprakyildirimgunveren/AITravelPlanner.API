using AITravelPlanner.Data;
using AITravelPlanner.Domain.Entities;
using AITravelPlanner.Services.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AITravelPlanner.Services.Services.Concrete
{
    public class FlightService : IFlightService
    {
        private readonly ApplicationDbContext _context;

        public FlightService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Flight>> SearchFlightsAsync(string from, string to)
        {
            return await _context.Flights
                .Where(f => f.From == from && f.To == to)
                .ToListAsync();
        }

        public async Task<Flight?> GetFlightByIdAsync(int id)
        {
            return await _context.Flights.FindAsync(id);
        }
    }
}
