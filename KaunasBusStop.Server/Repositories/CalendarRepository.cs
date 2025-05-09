using KaunasBusStop.Server.Enums;
using System.Formats.Asn1;
using System.Globalization;
using KaunasBusStop.Server.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

namespace KaunasBusStop.Server.Repositories
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly BusStopDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public CalendarRepository(IConfiguration configuration, BusStopDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task CreateAllCalendarsAsync()
        {
            try
            {
                await ImportCalendarsAsync(Path.Combine(_configuration["GTFS:DestinationFolder"], _configuration["GTFS:CalendarsFileName"]));
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("CreateAllCalendarsAsync failed: ", ex);
            }
        }

        private async Task ImportCalendarsAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) throw new Exception("ImportCalendarsAsync failed, filePath doesn't exist");

                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });

                var records = new List<Models.Calendar>();

                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var calendar = new Models.Calendar
                    {
                        ServiceId = csv.GetField<int>("service_id"),
                        Monday = string.IsNullOrEmpty(csv.GetField<string>("monday")) ? null : csv.GetField<WorkDay>("monday"),
                        Tuesday = string.IsNullOrEmpty(csv.GetField<string>("tuesday")) ? null : csv.GetField<WorkDay>("tuesday"),
                        Wednesday = string.IsNullOrEmpty(csv.GetField<string>("wednesday")) ? null : csv.GetField<WorkDay>("wednesday"),
                        Thursday = string.IsNullOrEmpty(csv.GetField<string>("thursday")) ? null : csv.GetField<WorkDay>("thursday"),
                        Friday = string.IsNullOrEmpty(csv.GetField<string>("friday")) ? null : csv.GetField<WorkDay>("friday"),
                        Saturday = string.IsNullOrEmpty(csv.GetField<string>("saturday")) ? null : csv.GetField<WorkDay>("saturday"),
                        Sunday = string.IsNullOrEmpty(csv.GetField<string>("sunday")) ? null : csv.GetField<WorkDay>("sunday"),
                        StartDate = DateOnly.ParseExact(csv.GetField<string>("start_date"), "yyyyMMdd"),
                        EndDate = DateOnly.ParseExact(csv.GetField<string>("end_date"), "yyyyMMdd")
                    };
                    records.Add(calendar);
                }
                await _dbContext.Calendars.AddRangeAsync(records);
            }
            catch (Exception ex)
            {
                throw new Exception("ImportCalendarsAsync failed: ", ex);
            }
        }

        public async Task<List<Models.Calendar>> GetCalendarsByServiceIdsAsync(List<int?> serviceIds)
        {
            try
            {
                return await _dbContext.Calendars
                    .Where(c => serviceIds.Contains(c.ServiceId))
                    .Select(c => new Models.Calendar { ServiceId = c.ServiceId, Monday = c.Monday, Tuesday = c.Tuesday, Wednesday = c.Wednesday, Thursday = c.Thursday, Friday = c.Friday, Saturday = c.Saturday, Sunday = c.Sunday })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("GetCalendarsByServiceIdsAsync failed: ", ex);
            }
        }

        public List<string> ConvertCalendarDaysToLetters(List<WorkDay> workDays)
        {
            var days = new List<string>() { "P", "A", "T", "K", "P", "Š", "S" };
            var result = new List<string>();

            for (int i = 0; i < workDays.Count; i++)
            {
                if (workDays[i] == WorkDay.ServiceAvailable)
                {
                    result.Add(days[i]);
                }
                else
                {
                    result.Add("0");
                }
            }

            return result;
        }
    }
}
