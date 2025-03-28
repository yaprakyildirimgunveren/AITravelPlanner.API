using AITravelPlanner.Domain.Entities;

namespace AITravelPlanner.Services.Services.Abstract
{
    public interface ITravelService
    {
        Task<List<Travel>> GetAllAsync();
        Task<Travel?> GetByIdAsync(int id);
        Task<Travel> CreateAsync(Travel travel);
        Task<Travel> UpdateAsync(Travel travel);
        Task<bool> DeleteAsync(int id);

        Task<List<Travel>> GetTravelsByUserAsync(int userId);
    }
}