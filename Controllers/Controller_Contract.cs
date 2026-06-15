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
    public class Controller_Contract : ControllerBase
    {
        private readonly IService_Contract _service;

        public Controller_Contract(IService_Contract service) { _service = service; }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
            => Ok(await _service.GetAllAsync(pageNumber, pageSize, search));

        [HttpGet("GetCurrent/{employeeCode}")]
        public async Task<IActionResult> GetCurrent(string employeeCode)
            => Ok(await _service.GetCurrentByCodeAsync(employeeCode));

        [HttpPost("Create")]
        [Authorize(Roles = "BlockManager")]
        public async Task<IActionResult> Create([FromBody] Request_CreateContract request)
            => Ok(await _service.CreateAsync(request));

        [HttpPut("Renew/{contractCode}")]
        [Authorize(Roles = "BlockManager")]
        public async Task<IActionResult> Renew(string contractCode, [FromBody] Request_RenewContract request)
            => Ok(await _service.RenewByCodeAsync(contractCode, request));

        [HttpPut("Terminate/{contractCode}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Terminate(string contractCode, [FromBody] Request_TerminateContract request)
            => Ok(await _service.TerminateByCodeAsync(contractCode, request));
    }
}