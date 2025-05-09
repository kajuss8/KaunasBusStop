using KaunasBusStop.Server.Interfaces;
using KaunasBusStop.Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace KaunasBusStop.Server.Controllers
{
    [ApiController]
    [Route("api/busStop/v1/[controller]")]
    public class StopScheduleController : ControllerBase
    {
        private readonly IStopScheduleRepository _stopScheduleRepository;
        public StopScheduleController(IStopScheduleRepository stopScheduleRepository)
        {
            _stopScheduleRepository = stopScheduleRepository;
        }

        [HttpGet("GetAllStopSchedule")]
        public async Task<List<StopSchedule>> GetStopScheduleByIdAsync(int id)
        {
            return await _stopScheduleRepository.GetStopScheduleByIdAsync(id);
        }
    }
}
