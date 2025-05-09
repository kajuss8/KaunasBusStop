using KaunasBusStop.Server.Models;

namespace KaunasBusStop.Server.Interfaces
{
    public interface IRouteScheduleRepository
    {
        Task<List<RouteSchedule>> GetRouteSchedules(string RouteId);
    }
}
