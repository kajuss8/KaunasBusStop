using KaunasBusStop.Server.Enums;
using System.Formats.Asn1;
using System.Globalization;
using KaunasBusStop.Server.Interfaces;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using CsvHelper.Configuration;

namespace KaunasBusStop.Server.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        private readonly BusStopDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public RouteRepository(BusStopDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task CreateAllRoutesAsync()
        {
            try
            {
                await ImportRoutesAsync(Path.Combine(_configuration["GTFS:DestinationFolder"], _configuration["GTFS:RoutesFileName"]));
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("CreateAllRoutesAsync failed: ", ex);
            }
        }

        public async Task<List<Models.Route>> GetAllRoutes()
        {
            return await _dbContext.Routes.ToListAsync();
        }

        private async Task ImportRoutesAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) throw new Exception("ImportRoutesAsync failed, filePath doesn't exist");

                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });

                var records = new List<Models.Route>();

                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var route = new Models.Route
                    {
                        RouteId = csv.GetField<string>("route_id"),
                        RouteShorName = string.IsNullOrEmpty(csv.GetField<string>("route_short_name")) ? null : csv.GetField<string>("route_short_name"),
                        RouteLongName = string.IsNullOrEmpty(csv.GetField<string>("route_long_name")) ? null : csv.GetField<string>("route_long_name"),
                        RouteDescription = string.IsNullOrEmpty(csv.GetField<string>("route_desc")) ? null : csv.GetField<string>("route_desc"),
                        RouteType = string.IsNullOrEmpty(csv.GetField<string>("route_type")) ? null : csv.GetField<TransportType>("route_type"),
                        RouteURL = string.IsNullOrEmpty(csv.GetField<string>("route_url")) ? null : csv.GetField<string>("route_url"),
                        RouteColor = string.IsNullOrEmpty(csv.GetField<string>("route_color")) ? null : csv.GetField<string>("route_color"),
                        RouteText = string.IsNullOrEmpty(csv.GetField<string>("route_text_color")) ? null : csv.GetField<string>("route_text_color"),
                        RouteSortOrder = string.IsNullOrEmpty(csv.GetField<string>("route_sort_order")) ? null : csv.GetField<int>("route_sort_order"),
                    };
                    records.Add(route);
                }
                await _dbContext.Routes.AddRangeAsync(records);
            }
            catch (Exception ex)
            {
                throw new Exception("ImportRoutesAsync failed: ", ex);
            }
        }

        public async Task<List<Models.Route>> GetRoutesByRouteIdsAsync(List<string?> routeIds)
        {
            try
            {
                return await _dbContext.Routes
                    .Where(r => routeIds.Contains(r.RouteId))
                    .Select(r => new Models.Route { RouteId = r.RouteId, RouteShorName = r.RouteShorName, RouteLongName = r.RouteLongName, RouteType = r.RouteType, RouteSortOrder = r.RouteSortOrder })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("GetRoutesByRouteIdsAsync failed: ", ex);
            }
        }
    }
}
