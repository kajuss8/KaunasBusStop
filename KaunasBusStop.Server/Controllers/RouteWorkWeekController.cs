using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace KaunasBusStop.Server.Controllers
{
    [ApiController]
    [Route("api/busStop/v1/[controller]")]
    public class RouteWorkWeekController : ControllerBase
    {
        private readonly IRouteWorkWeekRepository _routeWorkWeekRepository;
        public RouteWorkWeekController(IRouteWorkWeekRepository routeWorkWeekRepository)
        {
            _routeWorkWeekRepository = routeWorkWeekRepository;
        }

        [HttpPost("createAllRouteWorkWeek")]
        public async Task<IActionResult> CreateAllRoutesWorkWeek()
        {
            await _routeWorkWeekRepository.CreateAllRouteWorkWeekAsync();
            return Ok();
        }

        [HttpGet("getAllRouteWorkWeek")]
        public async Task<List<object>> GetAllRouteWorkWeek()
        {
            var routeWorkWeeks = await _routeWorkWeekRepository.GetAllRouteWorkWeekWithModifiedDataAsync();
            return routeWorkWeeks;
        }
    }
}
