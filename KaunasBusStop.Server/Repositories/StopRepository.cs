using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace KaunasBusStop.Server.Repositories
{
    public class StopRepository : IStopRepository
    {
        private readonly BusStopDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public StopRepository(BusStopDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task CreateAllStopsAsync()
        {
            try
            {
                await ImportStopsAsync(Path.Combine(_configuration["GTFS:DestinationFolder"], _configuration["GTFS:StopsFileName"]));
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("CreateAllStopsAsync failed: ", ex);
            }
        }

        

        public async Task<List<int>> GetAllStopIds()
        {
            return await _dbContext.Stops.Select(s => s.StopId).ToListAsync();
        }

        public async Task<string?> GetStopNameByIdAsync(int stopId)
        {
            try
            {
                return await _dbContext.Stops
                    .Where(s => s.StopId == stopId)
                    .Select(s => s.StopName)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("GetStopByIdAsync failed: ", ex);
            }
        }

        private async Task ImportStopsAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) throw new Exception("ImportCalendarsAsync failed, filePath doesn't exist");

                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });

                var records = new List<Stop>();

                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var stop = new Stop
                    {
                        StopId = csv.GetField<int>("stop_id"),
                        StopCode = string.IsNullOrEmpty(csv.GetField<string>("stop_code")) ? null : csv.GetField<string>("stop_code"),
                        StopName = string.IsNullOrEmpty(csv.GetField<string>("stop_name")) ? null : csv.GetField<string>("stop_name"),
                        StopDesc = string.IsNullOrEmpty(csv.GetField<string>("stop_desc")) ? null : csv.GetField<string>("stop_desc"),
                        StopLat = string.IsNullOrEmpty(csv.GetField<string>("stop_lat")) ? null : csv.GetField<float>("stop_lat"),
                        StopLon = string.IsNullOrEmpty(csv.GetField<string>("stop_lon")) ? null : csv.GetField<float>("stop_lon"),
                        StopURL = string.IsNullOrEmpty(csv.GetField<string>("stop_url")) ? null : csv.GetField<string>("stop_url"),
                        LocationType = string.IsNullOrEmpty(csv.GetField<string>("location_type")) ? null : csv.GetField<int>("location_type"),
                        ParentStation = string.IsNullOrEmpty(csv.GetField<string>("parent_station")) ? null : csv.GetField<int>("parent_station")
                    };
                    records.Add(stop);
                }
                await _dbContext.Stops.AddRangeAsync(records);
            }
            catch (Exception ex)
            {
                throw new Exception("ImportStopsAsync failed: ", ex);
            }
        }
        public async Task<List<Stop>> GetAllStopsAsync()
        {
            try
            {
                return await _dbContext.Stops.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("GetAllStopsAsync failed: ", ex);
            }
        }
    }
}
