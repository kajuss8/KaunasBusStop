using KaunasBusStop.Server.Enums;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace KaunasBusStop.Server.Repositories
{
    public class RouteScheduleRepository : IRouteScheduleRepository
    {
        private readonly ITripRepository _tripRepository;
        private readonly BusStopDbContext _dbcontext;
        public RouteScheduleRepository(ITripRepository tripRepository, BusStopDbContext busStopDbContext)
        {
            _tripRepository = tripRepository;
            _dbcontext = busStopDbContext;
        }

        public async Task<List<RouteSchedule>> GetRouteSchedules(string RouteId)
        {
            var routeSchedules = new List<RouteSchedule>();

            try
            {
                if (string.IsNullOrEmpty(RouteId))
                {
                    throw new ArgumentException("RouteId cannot be null or empty", nameof(RouteId));
                }

                var trips = await _tripRepository.GetTripsByRouteId(RouteId);
                if (trips == null)
                {
                    throw new Exception("CreateRouteSchedules failed: trips not found");
                }

                var stopTimesByTripId = await _dbcontext.StopTimes
                    .Where(st => trips.Select(t => t.TripId).Contains(st.TripId))
                    .ToListAsync();

                var TripsdifferentShapeIds = trips.Select(x => x.ShapeId).Distinct().ToList();
                var routeLongName = await _dbcontext.Routes
                    .Where(r => r.RouteId == RouteId)
                    .Select(r => r.RouteLongName)
                    .FirstOrDefaultAsync();

                if (routeLongName == null)
                {
                    throw new Exception("CreateRouteSchedules failed: route long name not found");
                }

                foreach (var shapeId in TripsdifferentShapeIds)
                {
                    var tripsByShapeId = trips.Where(t => t.ShapeId == shapeId).ToList();
                    var serviceIds = tripsByShapeId.Select(t => t.ServiceId).Distinct().ToList();
                    var routeInformations = new List<RouteInformation>();

                    foreach (var serviceId in serviceIds)
                    {
                        var tripsByServiceId = tripsByShapeId
                            .Where(t => t.ServiceId == serviceId)
                            .Select(t => t.TripId)
                            .ToList();

                        var stopTimes = stopTimesByTripId
                            .Where(st => tripsByServiceId.Contains(st.TripId))
                            .OrderBy(st => st.StopSequence)
                            .ToList();

                        var calendar = await _dbcontext.Calendars
                            .Where(c => c.ServiceId == serviceId)
                            .FirstOrDefaultAsync();

                        var workDays = new List<string>();
                        if (calendar != null)
                        {
                            if (calendar.Monday == WorkDay.ServiceAvailable) workDays.Add("Pirmadienis");
                            if (calendar.Tuesday == WorkDay.ServiceAvailable) workDays.Add("Antradienis");
                            if (calendar.Wednesday == WorkDay.ServiceAvailable) workDays.Add("Trečiadienis");
                            if (calendar.Thursday == WorkDay.ServiceAvailable) workDays.Add("Ketvirtadienis");
                            if (calendar.Friday == WorkDay.ServiceAvailable) workDays.Add("Penktadienis");
                            if (calendar.Saturday == WorkDay.ServiceAvailable) workDays.Add("Šeštadienis");
                            if (calendar.Sunday == WorkDay.ServiceAvailable) workDays.Add("Sekmadienis");
                        }

                        var stopIds = stopTimes.Select(st => st.StopId).Distinct().ToList();
                        var stops = new List<Stop>();

                        foreach (var stopId in stopIds)
                        {
                            var stop = await _dbcontext.Stops.FirstOrDefaultAsync(s => s.StopId == stopId);
                            if (stop != null)
                            {
                                stops.Add(stop);
                            }
                        }

                        var routeInformation = new RouteInformation { WorkDays = workDays };

                        foreach (var stop in stops)
                        {
                            var stopInfo = new Stopinfo
                            {
                                StopId = stop.StopId,
                                StopLat = stop.StopLat,
                                StopLon = stop.StopLon,
                                StopName = stop.StopName,
                                DepartureTime = stopTimes
                                    .Where(st => st.StopId == stop.StopId)
                                    .Select(d => d.DepartureTime)
                                    .ToList(),
                            };
                            routeInformation.Stopinfos.Add(stopInfo);
                        }

                        routeInformations.Add(routeInformation);
                    }

                    routeSchedules.Add(new RouteSchedule
                    {
                        RouteLongName = routeLongName,
                        ShapeId = shapeId,
                        RouteInformation = routeInformations
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("CreateRouteSchedule failed: ", ex);
            }

            return routeSchedules;
        }

    }
}
