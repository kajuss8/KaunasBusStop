using KaunasBusStop.Server.Models;

namespace KaunasBusStop.Server.Interfaces
{
    public interface IStopRepository
    {
        Task CreateAllStopsAsync();
        Task<List<int>> GetAllStopIds();
        Task<string> GetStopNameByIdAsync(int stopId);
        Task<List<Stop>> GetAllStopsAsync();
    }
}
