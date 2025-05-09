using KaunasBusStop.Server.Models;

namespace KaunasBusStop.Server.Interfaces
{
    public interface IStopScheduleRepository
    {
        Task<List<StopSchedule>> GetStopScheduleByIdAsync(int stopId);
    }
}
