using KaunasBusStop.Server.Models;

namespace KaunasBusStop.Server.Interfaces
{
    public interface IRouteWorkWeekRepository
    {
        Task CreateAllRouteWorkWeekAsync();
        Task<List<RouteWorkWeek>> GetAllRouteWorkWeek();
        Task<List<object>> GetAllRouteWorkWeekWithModifiedDataAsync();
    }
}
