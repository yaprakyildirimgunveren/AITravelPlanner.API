using AITravelPlanner.Data.Repositories.Interfaces;
using AITravelPlanner.Domain.Entities;

namespace AITravelPlanner.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepository<User> Users { get; private set; }
        public IGenericRepository<Flight> Flights { get; private set; }
        public IGenericRepository<Travel> Travels { get; private set; }
        public IGenericRepository<Activity> Activities { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new GenericRepository<User>(_context);
            Flights = new GenericRepository<Flight>(_context);
            Travels = new GenericRepository<Travel>(_context);
            Activities = new GenericRepository<Activity>(_context);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
