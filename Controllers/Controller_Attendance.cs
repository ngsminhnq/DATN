using HRemployee.PayLoad.Request;
using HRemployee.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRemployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Controller_Attendance : ControllerBase
    {
        private readonly IService_Attendance _service;

        public Controller_Attendance(IService_Attendance service) { _service = service; }

        [HttpPost("CheckIn")]
        public async Task<IActionResult> CheckIn([FromBody] Request_CheckIn request)
        {
            int employeeId = GetEmployeeId();
            if (employeeId == 0) return Unauthorized("Không xác định được nhân viên từ token");
            return Ok(await _service.CheckInAsync(employeeId, request));
        }

        [HttpPost("CheckOut")]
        public async Task<IActionResult> CheckOut([FromBody] Request_CheckOut request)
        {
            int employeeId = GetEmployeeId();
            if (employeeId == 0) return Unauthorized("Không xác định được nhân viên từ token");
            return Ok(await _service.CheckOutAsync(employeeId, request));
        }

        [HttpGet("TodayStatus")]
        public async Task<IActionResult> TodayStatus()
        {
            int employeeId = GetEmployeeId();
            return Ok(await _service.GetTodayStatusAsync(employeeId));
        }

        [HttpGet("MyHistory")]
        public async Task<IActionResult> MyHistory([FromQuery] int month, [FromQuery] int year)
        {
            int employeeId = GetEmployeeId();
            return Ok(await _service.GetByEmployeeAsync(employeeId, month, year));
        }

        [HttpGet("GetByEmployee/{employeeCode}")]
        [Authorize(Roles = "BlockManager,CenterManager")]
        public async Task<IActionResult> GetByEmployee(string employeeCode, [FromQuery] int month, [FromQuery] int year)
            => Ok(await _service.GetByEmployeeByCodeAsync(employeeCode, month, year));

        [HttpGet("MySummary")]
        public async Task<IActionResult> MySummary([FromQuery] int month, [FromQuery] int year)
        {
            int employeeId = GetEmployeeId();
            return Ok(await _service.GetSummaryAsync(employeeId, month, year));
        }

        [HttpGet("Summary/{employeeCode}")]
        [Authorize(Roles = "BlockManager,CenterManager")]
        public async Task<IActionResult> Summary(string employeeCode, [FromQuery] int month, [FromQuery] int year)
            => Ok(await _service.GetSummaryByCodeAsync(employeeCode, month, year));

        private int GetEmployeeId()
        {
            var claim = HttpContext.User.FindFirst("EmployeeId");
            return int.TryParse(claim?.Value, out int id) ? id : 0;
        }

        [HttpPut("Edit/{employeeCode}")]
        [Authorize(Roles = "CenterManager")]
        public async Task<IActionResult> Edit(string employeeCode, [FromQuery] string date, [FromBody] Request_EditAttendance request)
            => Ok(await _service.EditByCodeDateAsync(employeeCode, date, request));

        [HttpPost("CalculateMonthly")]
        [Authorize(Roles = "BlockManager")]
        public async Task<IActionResult> CalculateMonthly([FromQuery] string employeeCode, [FromQuery] int month, [FromQuery] int year)
            => Ok(await _service.CalculateMonthlyByCodeAsync(employeeCode, month, year));
    }
}