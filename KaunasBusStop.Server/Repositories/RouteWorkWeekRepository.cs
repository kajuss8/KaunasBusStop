using KaunasBusStop.Server.Enums;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace KaunasBusStop.Server.Repositories
{
    public class RouteWorkWeekRepository : IRouteWorkWeekRepository
    {
        private readonly BusStopDbContext _dbContext;
        public RouteWorkWeekRepository(BusStopDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateAllRouteWorkWeekAsync()
        {
            try
            {
                var routeWorkWeeks = (from route in _dbContext.Routes
                                      join trip in _dbContext.Trips on route.RouteId equals trip.RouteId
                                      join calendar in _dbContext.Calendars on trip.ServiceId equals calendar.ServiceId
                                      group calendar by new
                                      {
                                          route.RouteId,
                                          route.RouteShorName,
                                          route.RouteLongName,
                                          route.RouteType,
                                          route.RouteSortOrder
                                      } into g
                                      select new RouteWorkWeek
                                      {
                                          RouteId = g.Key.RouteId,
                                          RouteShorName = g.Key.RouteShorName,
                                          RouteLongName = g.Key.RouteLongName,
                                          RouteType = g.Key.RouteType,
                                          RouteSortOrder = g.Key.RouteSortOrder,
                                          Monday = g.Max(c => c.Monday),
                                          Tuesday = g.Max(c => c.Tuesday),
                                          Wednesday = g.Max(c => c.Wednesday),
                                          Thursday = g.Max(c => c.Thursday),
                                          Friday = g.Max(c => c.Friday),
                                          Saturday = g.Max(c => c.Saturday),
                                          Sunday = g.Max(c => c.Sunday)
                                      }).OrderBy(r => r.RouteSortOrder).ToList();

                if (routeWorkWeeks.Any())
                {
                    await _dbContext.RoutesWorkWeeks.AddRangeAsync(routeWorkWeeks);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("CreateAllRouteWorkWeekAsync failed, no routeWorkWeek");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("CreateAllRouteWorkWeekAsync failed: ", ex);
            }
        }

        public async Task<List<RouteWorkWeek>> GetAllRouteWorkWeek()
        {
            return await _dbContext.RoutesWorkWeeks.ToListAsync();
        }

        public async Task<List<object>> GetAllRouteWorkWeekWithModifiedDataAsync()
        {
            var routeWorkWeeks = await GetAllRouteWorkWeek();

            var result = routeWorkWeeks.Select(r => new
            {
                RouteId = r.RouteId,
                RouteShorName = r.RouteShorName,
                RouteLongName = r.RouteLongName,
                RouteType = r.RouteType == TransportType.Bus ? (string)"A" : r.RouteType == TransportType.Trolleybus ? (string)"T" : null,
                RouteSortOrder = r.RouteSortOrder,
                ActiveDays = new List<string>
                {
                    r.Monday == WorkDay.ServiceAvailable ? "P" : null,
                    r.Tuesday == WorkDay.ServiceAvailable ? "A" : null,
                    r.Wednesday == WorkDay.ServiceAvailable ? "T" : null,
                    r.Thursday == WorkDay.ServiceAvailable ? "K" : null,
                    r.Friday == WorkDay.ServiceAvailable ? "P" : null,
                    r.Saturday == WorkDay.ServiceAvailable ? "Š" : null,
                    r.Sunday == WorkDay.ServiceAvailable ? "S" : null
                }.Where(day => day != null).ToList()
            }).ToList();

            return result.Cast<object>().ToList();
        }
    }
}
