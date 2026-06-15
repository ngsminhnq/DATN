using HRemployee.PayLoad.Request;
using HRemployee.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRemployee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Controller_SalaryConfig : ControllerBase
    {
        private readonly IService_SalaryConfig _service;

        public Controller_SalaryConfig(IService_SalaryConfig service)
            => _service = service;

        [HttpGet("GetAll")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
            => Ok(await _service.GetAllAsync(pageNumber, pageSize, search));

        [HttpPost("Create/{employeeCode}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Create(string employeeCode, [FromBody] Request_SalaryConfig request)
            => Ok(await _service.CreateAsync(employeeCode, request));

        [HttpPut("Update/{employeeCode}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Update(string employeeCode, [FromBody] Request_SalaryConfig request)
            => Ok(await _service.UpdateAsync(employeeCode, request));

        [HttpGet("GetCurrent/{employeeCode}")]
        [Authorize(Roles = "Director,BlockManager,CenterManager")]
        public async Task<IActionResult> GetCurrent(string employeeCode)
            => Ok(await _service.GetCurrentAsync(employeeCode));

        [HttpGet("GetHistory/{employeeCode}")]
        [Authorize(Roles = "Director,BlockManager,CenterManager")]
        public async Task<IActionResult> GetHistory(string employeeCode)
            => Ok(await _service.GetHistoryAsync(employeeCode));
    }
}