using HRemployee.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRemployee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Director,BlockManager,CenterManager")]
    public class Controller_Statistics : ControllerBase
    {
        private readonly IService_Statistics _service;

        public Controller_Statistics(IService_Statistics service)
            => _service = service;

        [HttpGet("Employees")]
        public async Task<IActionResult> GetEmployeeStats()
            => Ok(await _service.GetEmployeeStatsAsync());

        [HttpGet("Attendance")]
        public async Task<IActionResult> GetAttendanceStats(
            [FromQuery] int month,
            [FromQuery] int year)
            => Ok(await _service.GetAttendanceStatsAsync(month, year));

        [HttpGet("Salary")]
        public async Task<IActionResult> GetSalaryStats(
            [FromQuery] int month,
            [FromQuery] int year)
            => Ok(await _service.GetSalaryStatsAsync(month, year));
    }
}