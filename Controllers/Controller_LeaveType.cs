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
    public class Controller_LeaveType : ControllerBase
    {
        private readonly IService_LeaveType _service;

        public Controller_LeaveType(IService_LeaveType service) => _service = service;

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] string? search = null)
            => Ok(await _service.GetAllAsync(search));

        [HttpPost("Create")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Create([FromBody] Request_CreateLeaveType request)
            => Ok(await _service.CreateAsync(request));

        [HttpPut("Update/{id}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Update(int id, [FromBody] Request_UpdateLeaveType request)
            => Ok(await _service.UpdateAsync(id, request));

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Delete(int id)
            => Ok(await _service.DeleteAsync(id));
    }
}