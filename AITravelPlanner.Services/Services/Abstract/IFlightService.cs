using AITravelPlanner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AITravelPlanner.Services.Services.Abstract
{
    public interface IFlightService
    {
        Task<Flight?> GetFlightByIdAsync(int id);
        Task<List<Flight>> SearchFlightsAsync(string from, string to);
    }
}
