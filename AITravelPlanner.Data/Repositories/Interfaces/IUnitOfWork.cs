using AITravelPlanner.Domain.Entities;

namespace AITravelPlanner.Data.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Flight> Flights { get; }
        IGenericRepository<Travel> Travels { get; }
        IGenericRepository<Activity> Activities { get; }

        Task<int> SaveAsync();
    }
}
