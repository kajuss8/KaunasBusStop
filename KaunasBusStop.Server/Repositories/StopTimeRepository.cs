using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace KaunasBusStop.Server.Repositories
{
    public class StopTimeRepository : IStopTimeRepository
    {
        private readonly BusStopDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public StopTimeRepository(BusStopDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }
        public async Task CreateAllStopTimesAsync()
        {
            try
            {
                await ImportStopTimesAsync(Path.Combine(_configuration["GTFS:DestinationFolder"], _configuration["GTFS:StopTimesFileName"]));
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("CreateAllStopTimesAsync failed: ", ex);
            }
        }

        public async Task<List<StopTime>> GetAllStopTimesByStopid(int stopId)
        {
            try
            {
                var stopTimes = await _dbContext.StopTimes.Where(st => st.StopId == stopId).ToListAsync();

                if (stopTimes.IsNullOrEmpty())
                {
                    throw new Exception("GetAllStopTimesByStopid failed, stopTImes is null or empty");
                }
                return stopTimes;
            }
            catch (Exception ex)
            {
                throw new Exception("GetAllStopTimesByStopid failed", ex);
            }
        }

        private async Task ImportStopTimesAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) throw new Exception("ImportStopTimesAsync failed, filePath doesn't exist");

                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });

                var records = new List<StopTime>();

                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var stopTime = new StopTime
                    {
                        TripId = string.IsNullOrEmpty(csv.GetField<string>("trip_id")) ? null : csv.GetField<string>("trip_id"),
                        ArrivalTime = string.IsNullOrEmpty(csv.GetField<string>("arrival_time")) ? null : ParseTime(csv.GetField<string>("arrival_time")),
                        DepartureTime = string.IsNullOrEmpty(csv.GetField<string>("departure_time")) ? null : ParseTime(csv.GetField<string>("departure_time")),
                        StopId = string.IsNullOrEmpty(csv.GetField<string>("stop_id")) ? null : csv.GetField<int>("stop_id"),
                        StopSequence = string.IsNullOrEmpty(csv.GetField<string>("stop_sequence")) ? null : csv.GetField<int>("stop_sequence"),
                        PickUpType = string.IsNullOrEmpty(csv.GetField<string>("pickup_type")) ? null : csv.GetField<int>("pickup_type"),
                        DropOffType = string.IsNullOrEmpty(csv.GetField<string>("drop_off_type")) ? null : csv.GetField<int>("drop_off_type"),
                    };
                    records.Add(stopTime);
                }
                await _dbContext.StopTimes.AddRangeAsync(records);
            }
            catch (Exception ex)
            {
                throw new Exception("ImportStopTimesAsync failed: ", ex);
            }
        }

        private string ParseTime(string timeStr)
        {
            if (string.IsNullOrEmpty(timeStr))
                throw new FormatException("Time cannot be null or empty");

            var parts = timeStr.Split(':');
            if (parts.Length != 3)
                throw new FormatException($"Invalid time format: {timeStr}");

            string hours = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];

            int tempHours = int.Parse(hours);
            if (tempHours > 24)
            {
                tempHours -= 24;
                hours = $"0{tempHours}";
            }
            else if (tempHours == 24)
            {
                hours = "00";
            }

            string normalizedTime = $"{hours}:{minutes}";

            return normalizedTime;
        }

        public async Task<List<StopTime>> GetStopTimesByStopIdAsync(int stopId)
        {
            try
            {
                return await _dbContext.StopTimes
                    .Where(st => st.StopId == stopId)
                    .Select(st => new StopTime { TripId = st.TripId, ArrivalTime = st.ArrivalTime })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("GetStopTimesByStopIdAsync failed: ", ex);
            }
        }
    }
}
