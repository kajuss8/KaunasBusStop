using KaunasBusStop.Server.Models;

namespace KaunasBusStop.Server.Interfaces
{
    public interface IStopTimeRepository
    {
        Task CreateAllStopTimesAsync();
        Task<List<StopTime>> GetAllStopTimesByStopid(int stopId);
        Task<List<StopTime>> GetStopTimesByStopIdAsync(int stopId);
    }
}
