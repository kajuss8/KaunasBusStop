using KaunasBusStop.Server.Enums;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;

namespace KaunasBusStop.Server.Repositories
{
    public class StopScheduleRepository : IStopScheduleRepository
    {
        private readonly IStopRepository _stopRepository;
        private readonly IStopTimeRepository _stopTimeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly ICalendarRepository _calendarRepository;
        public StopScheduleRepository(BusStopDbContext dbContext, IStopRepository stopRepository, IStopTimeRepository stopTimeRepository,
            ITripRepository tripRepository, IRouteRepository routeRepository, ICalendarRepository calendarRepository)
        {
            _stopRepository = stopRepository;
            _stopTimeRepository = stopTimeRepository;
            _tripRepository = tripRepository;
            _routeRepository = routeRepository;
            _calendarRepository = calendarRepository;
        }

        public async Task<List<StopSchedule>> GetStopScheduleByIdAsync(int stopId)
        {
            try
            {
                var stop = await _stopRepository.GetStopNameByIdAsync(stopId);
                if (stop == null)
                {
                    throw new Exception("Stop not found.");
                }

                var stopTimes = await _stopTimeRepository.GetStopTimesByStopIdAsync(stopId);
                if (stopTimes == null)
                {
                    throw new Exception("No StopTimes found for this StopId.");
                }

                var tripIds = stopTimes.Select(st => st.TripId).Distinct().ToList();
                var trips = await _tripRepository.GetTripsByTripIdsAsync(tripIds);
                if (trips == null)
                {
                    throw new Exception("No Trips found for the given TripIds.");
                }

                var routeIds = trips.Select(t => t.RouteId).Distinct().ToList();
                var routes = await _routeRepository.GetRoutesByRouteIdsAsync(routeIds);
                if (routes == null)
                {
                    throw new Exception("No Routes found for the given RouteIds.");
                }

                var serviceIds = trips.Select(t => t.ServiceId).Distinct().ToList();
                var calendars = await _calendarRepository.GetCalendarsByServiceIdsAsync(serviceIds);
                if (calendars == null)
                {
                    throw new Exception("No Calendars found for the given ServiceIds.");
                }

                var groupedResult = GroupAndSortResults(stop, stopTimes, trips, routes, calendars);

                return groupedResult;
            }
            catch (Exception ex)
            {
                throw new Exception("GetStopScheduleByIdAsync failed: ", ex);
            }
        }

        private List<StopSchedule> GroupAndSortResults(string stopName, List<StopTime> stopTimes, List<Trip> trips, List<Models.Route> routes, List<Calendar> calendars)
        {
            try
            {
                var groupedResult = (from st in stopTimes
                                     join t in trips on st.TripId equals t.TripId
                                     join r in routes on t.RouteId equals r.RouteId
                                     join c in calendars on t.ServiceId equals c.ServiceId
                                     group st.ArrivalTime by new
                                     {
                                         t.ShapeId,
                                         t.RouteId,
                                         r.RouteShorName,
                                         r.RouteLongName,
                                         r.RouteType,
                                         r.RouteSortOrder,
                                         c.Monday,
                                         c.Tuesday,
                                         c.Wednesday,
                                         c.Thursday,
                                         c.Friday,
                                         c.Saturday,
                                         c.Sunday
                                     } into grouped
                                     select new StopInformation
                                     {
                                         ShapeId = grouped.Key.ShapeId,
                                         RouteId = grouped.Key.RouteId,
                                         RouteShorName = grouped.Key.RouteShorName,
                                         RouteLongName = grouped.Key.RouteLongName,
                                         RouteType = grouped.Key.RouteType,
                                         RouteSortOrder = grouped.Key.RouteSortOrder,
                                         WorkDays = new List<string>()
                                        {
                                            grouped.Key.Monday == WorkDay.ServiceAvailable ? "P" : null,
                                            grouped.Key.Tuesday == WorkDay.ServiceAvailable ? "A" : null,
                                            grouped.Key.Wednesday == WorkDay.ServiceAvailable ? "T" : null,
                                            grouped.Key.Thursday == WorkDay.ServiceAvailable ? "K" : null,
                                            grouped.Key.Friday == WorkDay.ServiceAvailable ? "P" : null,
                                            grouped.Key.Saturday == WorkDay.ServiceAvailable ? "Š" : null,
                                            grouped.Key.Sunday == WorkDay.ServiceAvailable ? "S" : null
                                        }.Where(day => day != null).ToList(),
                                         ArrivalTime = grouped.Select(at => at).OrderBy(at => at).ToList()
                                     })
                                    .OrderBy(r => r.RouteSortOrder)
                                    .ThenBy(r => r.ShapeId)
                                    .ToList();

                return new List<StopSchedule>
               {
                   new StopSchedule
                   {
                       StopName = stopName,
                       StopInformation = groupedResult
                   }
               };
            }
            catch (Exception ex)
            {
                throw new Exception("GroupAndSortResults failed: ", ex);
            }
        }
    }
}
