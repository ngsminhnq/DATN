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
    public class Controller_Employee : ControllerBase
    {
        private readonly IService_Employee _service;

        public Controller_Employee(IService_Employee service) { _service = service; }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
            => Ok(await _service.GetAllAsync(pageNumber, pageSize, search));

        [HttpPost("Create")]
        [Authorize(Roles = "BlockManager")]
        public async Task<IActionResult> Create([FromBody] Request_CreateEmployee request)
            => Ok(await _service.CreateAsync(request));

        [HttpPut("Update/{employeeCode}")]
        [Authorize(Roles = "BlockManager")]
        public async Task<IActionResult> Update(string employeeCode, [FromBody] Request_UpdateEmployee request)
            => Ok(await _service.UpdateByCodeAsync(employeeCode, request));

        [HttpPost("UploadAvatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            var employeeIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EmployeeId")?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int employeeId))
                return Unauthorized(new { status = 401, message = "Không xác định được tài khoản đăng nhập" });

            return Ok(await _service.UploadAvatarAsync(employeeId, avatar));
        }

        [HttpDelete("Delete/{employeeCode}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Delete(string employeeCode)
            => Ok(await _service.DeleteByCodeAsync(employeeCode));
    }
}