using AITravelPlanner.Data;
using AITravelPlanner.Domain.Entities;
using AITravelPlanner.Services.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AITravelPlanner.Services.Services.Concrete
{
    public class TravelService : ITravelService
    {
        private readonly ApplicationDbContext _context;

        public TravelService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Travel>> GetAllAsync()
        {
            return await _context.Travels.Include(t => t.Activities).ToListAsync();
        }

        public async Task<Travel?> GetByIdAsync(int id)
        {
            return await _context.Travels.Include(t => t.Activities).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Travel> CreateAsync(Travel travel)
        {
            _context.Travels.Add(travel);
            await _context.SaveChangesAsync();
            return travel;
        }

        public async Task<Travel> UpdateAsync(Travel travel)
        {
            _context.Travels.Update(travel);
            await _context.SaveChangesAsync();
            return travel;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var travel = await _context.Travels.FindAsync(id);
            if (travel == null) return false;

            _context.Travels.Remove(travel);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Travel>> GetTravelsByUserAsync(int userId)
        {
            return await _context.Travels.Where(t => t.UserId == userId).ToListAsync();
        }
    }
}