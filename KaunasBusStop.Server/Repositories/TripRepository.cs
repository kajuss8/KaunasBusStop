using KaunasBusStop.Server.Enums;
using System.Formats.Asn1;
using System.Globalization;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using CsvHelper.Configuration;

namespace KaunasBusStop.Server.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly BusStopDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public TripRepository(BusStopDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task CreateAllTripsAsync()
        {
            try
            {
                await ImportTripsAsync(Path.Combine(_configuration["GTFS:DestinationFolder"], _configuration["GTFS:TripsFileName"]));
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("CreateAllTripsAsync failed: ", ex);
            }
        }



        private async Task ImportTripsAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) throw new Exception("ImportTripsAsync failed, filePath doesn't exist");

                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });

                var records = new List<Trip>();

                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var trip = new Trip
                    {
                        RouteId = string.IsNullOrEmpty(csv.GetField<string>("route_id")) ? null : csv.GetField<string>("route_id"),
                        ServiceId = string.IsNullOrEmpty(csv.GetField<string>("service_id")) ? null : csv.GetField<int>("service_id"),
                        TripId = csv.GetField<string>("trip_id"),
                        TripHeadsign = string.IsNullOrEmpty(csv.GetField<string>("trip_headsign")) ? null : csv.GetField<string>("trip_headsign"),
                        DirectionId = string.IsNullOrEmpty(csv.GetField<string>("direction_id")) ? null : csv.GetField<Direction>("direction_id"),
                        BlockId = string.IsNullOrEmpty(csv.GetField<string>("block_id")) ? null : csv.GetField<int>("block_id"),
                        ShapeId = string.IsNullOrEmpty(csv.GetField<string>("shape_id")) ? null : csv.GetField<string>("shape_id"),
                        WheelchairAccessible = string.IsNullOrEmpty(csv.GetField<string>("wheelchair_accessible")) ? null : csv.GetField<int>("wheelchair_accessible"),

                    };
                    records.Add(trip);
                }
                await _dbContext.Trips.AddRangeAsync(records);
            }
            catch (Exception ex)
            {
                throw new Exception("ImportTripsAsync failed: ", ex);
            }
        }

        public async Task<List<Trip>> GetTripsByTripIdsAsync(List<string?> tripIds)
        {
            try
            {
                return await _dbContext.Trips
                    .Where(t => tripIds.Contains(t.TripId))
                    .Select(t => new Trip { TripId = t.TripId, RouteId = t.RouteId, ServiceId = t.ServiceId, ShapeId = t.ShapeId })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("GetTripsByTripIdsAsync failed: ", ex);
            }
        }

        public async Task<List<Trip>> GetTripsByRouteId(string RouteId)
        {
            try
            {
                return await _dbContext.Trips
                .Where(t => t.RouteId == RouteId)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("GetTripsByTripIdsAsync failed: ", ex);
            }
        }
    }
}
