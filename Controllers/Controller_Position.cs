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
    public class Controller_Position : ControllerBase
    {
        private readonly IService_Position _service;

        public Controller_Position(IService_Position service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
            => Ok(await _service.GetAllAsync(pageNumber, pageSize, search));

        [HttpPost("Create")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Create([FromBody] Request_CreatePosition request)
            => Ok(await _service.CreateAsync(request));

        [HttpPut("Update/{code}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Update(string code, [FromBody] Request_UpdatePosition request)
            => Ok(await _service.UpdateByCodeAsync(code, request));

        [HttpDelete("Delete/{code}")]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> Delete(string code)
            => Ok(await _service.DeleteByCodeAsync(code));
    }
}