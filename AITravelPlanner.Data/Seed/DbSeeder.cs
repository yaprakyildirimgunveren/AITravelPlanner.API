using AITravelPlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AITravelPlanner.Data.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync())
            {
                return;
            }

            var user = new User
            {
                Name = "Yaprak",
                SurName = "Yildirim",
                UserName = "yaprak",
                Email = "yaprak@example.com",
                PasswordHash = "demo-hash",
                PhoneNumber = "+90-555-000-0000"
            };

            var travel = new Travel
            {
                Destination = "Athens",
                StartDate = DateTime.UtcNow.Date.AddDays(30),
                EndDate = DateTime.UtcNow.Date.AddDays(35),
                Type = TravelType.Leisure,
                Budget = 1500,
                User = user
            };

            var activities = new List<Activity>
            {
                new Activity
                {
                    Name = "Acropolis Tour",
                    Description = "Guided historical tour",
                    ActivityDate = DateTime.UtcNow.Date.AddDays(31),
                    Location = "Acropolis",
                    Price = 120,
                    Category = "Sightseeing",
                    Travel = travel
                },
                new Activity
                {
                    Name = "Food Walk",
                    Description = "Local cuisine tasting",
                    ActivityDate = DateTime.UtcNow.Date.AddDays(32),
                    Location = "Plaka",
                    Price = 80,
                    Category = "Food",
                    Travel = travel
                }
            };

            var flights = new List<Flight>
            {
                new Flight
                {
                    From = "IST",
                    To = "ATH",
                    DepartureDate = DateTime.UtcNow.Date.AddDays(30),
                    Price = 220
                },
                new Flight
                {
                    From = "IST",
                    To = "ROM",
                    DepartureDate = DateTime.UtcNow.Date.AddDays(45),
                    Price = 280
                }
            };

            context.Users.Add(user);
            context.Travels.Add(travel);
            context.Activities.AddRange(activities);
            context.Flights.AddRange(flights);

            await context.SaveChangesAsync();
        }
    }
}
