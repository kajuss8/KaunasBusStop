using KaunasBusStop.Server.HandleGTFS;
using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace KaunasBusStop.Server.Controllers
{
    [ApiController]
    [Route("api/busStop/v1/[controller]")]
    public class GTFSController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStopRepository _stopRepository;
        private readonly ICalendarRepository _calendarRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IStopTimeRepository _stopTimeRepository;
        private readonly IRouteWorkWeekRepository _routeWorkWeekRepository;
        public GTFSController(IConfiguration configuration, IStopRepository stopRepository, ICalendarRepository calendarRepository,
            IRouteRepository routeRepository, ITripRepository tripRepository, IStopTimeRepository stopTimeRepository, IRouteWorkWeekRepository routeWorkWeekRepository)
        {
            _configuration = configuration;
            _stopRepository = stopRepository;
            _calendarRepository = calendarRepository;
            _routeRepository = routeRepository;
            _tripRepository = tripRepository;
            _stopTimeRepository = stopTimeRepository;
            _routeWorkWeekRepository = routeWorkWeekRepository;
        }

        [HttpGet("downloadGTFS")]
        public async Task<IActionResult> GetGTFSFiles()
        {
            var url = _configuration["GTFS:URL"];
            var destinationFolder = Path.Combine(Directory.GetCurrentDirectory(), _configuration["GTFS:DestinationFolder"]!);

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(destinationFolder))
            {
                return BadRequest("Invalid configuration for GTFS download.");
            }

            var downloader = new GTFSDownloader();
            await downloader.DownloadAndUnzipGTFSFileAsync(url, destinationFolder);

            return Ok();
        }

        [HttpPost("importGTFS")]
        public async Task<IActionResult> DownloadAndImportGTFS()
        {
            await _stopRepository.CreateAllStopsAsync();
            await _calendarRepository.CreateAllCalendarsAsync();
            await _routeRepository.CreateAllRoutesAsync();
            await _tripRepository.CreateAllTripsAsync();
            await _stopTimeRepository.CreateAllStopTimesAsync();
            await _routeWorkWeekRepository.CreateAllRouteWorkWeekAsync();

            return Ok();
        }

        [HttpPost("createAllStops")]
        public async Task<IActionResult> ImportStopsToDatabase()
        {
            await _stopRepository.CreateAllStopsAsync();
            return Ok();
        }

        [HttpPost("createAllCalendars")]
        public async Task<IActionResult> ImportCalendarsToDatabase()
        {
            await _calendarRepository.CreateAllCalendarsAsync();
            return Ok();
        }

        [HttpPost("createAllRoutes")]
        public async Task<IActionResult> ImportRoutesToDatabase()
        {
            await _routeRepository.CreateAllRoutesAsync();
            return Ok();
        }

        [HttpPost("createAllTrips")]
        public async Task<IActionResult> ImportTripsToDatabase()
        {
            await _tripRepository.CreateAllTripsAsync();
            return Ok();
        }

        [HttpPost("createAllStopTimes")]
        public async Task<IActionResult> ImportStopTimesToDatabase()
        {
            await _stopTimeRepository.CreateAllStopTimesAsync();
            return Ok();
        }

        [HttpPost("CreateRouteWorkWeek")]
        public async Task<IActionResult> CreateRouteWorkWeek()
        {
            await _routeWorkWeekRepository.CreateAllRouteWorkWeekAsync();
            return Ok();
        }


        [HttpGet("getAllRoutes")]
        public async Task<List<Models.Route>> GetAllRoutesFromDatabase()
        {
            return await _routeRepository.GetAllRoutes();
        }

        [HttpGet("getAllStops")]
        public async Task<List<Stop>> GetAllStopsFromDatabase()
        {
            return await _stopRepository.GetAllStopsAsync();
        }

        [HttpGet("StopName")]
        public async Task<string> GetStopName(int id)
        {
            return await _stopRepository.GetStopNameByIdAsync(id);
        }

        [HttpGet("StopTimesByStopid")]
        public async Task<List<StopTime>> GetAllStopTimesByStopId(int id)
        {
            return await _stopTimeRepository.GetAllStopTimesByStopid(id);
        }
    }
}
