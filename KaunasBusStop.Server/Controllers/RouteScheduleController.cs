using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace KaunasBusStop.Server.Controllers
{
    [ApiController]
    [Route("api/busStop/v1/[controller]")]
    public class RouteScheduleController : ControllerBase
    {
        private readonly IRouteScheduleRepository _routeScheduleRepository;
        public RouteScheduleController(IRouteScheduleRepository routeScheduleRepository)
        {
            _routeScheduleRepository = routeScheduleRepository;
        }

        [HttpGet("GetRouteSchedulesByRouteId")]
        public async Task<List<RouteSchedule>> CreateRouteSchedulesAsync(string id)
        {
            return await _routeScheduleRepository.GetRouteSchedules(id);
        }


    }
}
