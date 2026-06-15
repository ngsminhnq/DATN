using HRemployee.PayLoad.Request;
using HRemployee.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRemployee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Controller_SalaryRecord : ControllerBase
    {
        private readonly IService_SalaryRecord _service;

        public Controller_SalaryRecord(IService_SalaryRecord service)
            => _service = service;

        [HttpPost("Calculate/{employeeCode}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Calculate(
            string employeeCode,
            [FromQuery] int month,
            [FromQuery] int year,
            [FromBody] Request_CalculateSalary request)
            => Ok(await _service.CalculateAsync(employeeCode, month, year, request));

        [HttpPost("CalculateAll")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> CalculateAll(
            [FromQuery] int month,
            [FromQuery] int year,
            [FromQuery] decimal kpiCoefficient = 1.0m)
            => Ok(await _service.CalculateAllAsync(month, year, kpiCoefficient));

        [HttpGet("GetMy")]
        [Authorize]
        public async Task<IActionResult> GetMy(
            [FromQuery] int month,
            [FromQuery] int year)
        {

            var empIdClaim = User.FindFirst("EmployeeId")?.Value ?? User.FindFirst("employeeId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(empIdClaim, out int empId))
                return Unauthorized(new { status = 401, message = "Không xác định được nhân viên từ token." });

            return Ok(await _service.GetMyAsync(empId, month, year));
        }

        [HttpGet("GetByEmployee/{employeeCode}")]
        [Authorize(Roles = "Director,BlockManager,CenterManager")]
        public async Task<IActionResult> GetByEmployee(string employeeCode)
            => Ok(await _service.GetByEmployeeAsync(employeeCode));

        [HttpGet("GetAll")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? month = null,
            [FromQuery] int? year = null)
            => Ok(await _service.GetAllAsync(pageNumber, pageSize, search, month, year));

        [HttpPost("SendEmail/{employeeCode}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> SendEmail(
            string employeeCode,
            [FromQuery] int month,
            [FromQuery] int year)
            => Ok(await _service.SendPayslipEmailAsync(employeeCode, month, year));
    }
}