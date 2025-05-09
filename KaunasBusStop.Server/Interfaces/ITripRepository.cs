using KaunasBusStop.Server.Models;

namespace KaunasBusStop.Server.Interfaces
{
    public interface ITripRepository
    {
        Task CreateAllTripsAsync();
        Task<List<Trip>> GetTripsByTripIdsAsync(List<string?> tripIds);
        Task<List<Trip>> GetTripsByRouteId(string RouteId);
    }
}
