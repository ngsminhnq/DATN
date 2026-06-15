using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRemployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controller_Auth : ControllerBase
    {
        private readonly IService_Authentic _service;

        public Controller_Auth(IService_Authentic service)
        {
            _service = service;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] Request_Login request)
            => Ok(await _service.Login(request));

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken([FromBody] DTO_Token request)
            => Ok(await _service.RenewAccessToken(request));

        [HttpPut("ChangePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromForm] Request_ChangePassword request)
        {
            if (!HttpContext.User.Identity!.IsAuthenticated)
                return Ok("Vui lòng đăng nhập!");

            int userId = int.Parse(HttpContext.User.FindFirst("Id")!.Value);
            return Ok(await _service.ChangePassword(request, userId));
        }

        [HttpGet("DecodeToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DecodeToken([FromQuery] string token)
            => Ok(await _service.DecodeJwtTokenAsync(token));

        [HttpPost("CreateAccount")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> CreateAccount([FromBody] Request_CreateUser request)
            => Ok(await _service.CreateAccount(request));

        [HttpPut("LockAccount/{employeeCode}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> LockAccount(string employeeCode, [FromQuery] bool isLocked = true)
            => Ok(await _service.LockAccountByEmployeeCode(employeeCode, isLocked));

        [HttpPut("ResetPassword/{employeeCode}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Director")]
        public async Task<IActionResult> ResetPassword(string employeeCode)
            => Ok(await _service.ResetPasswordByEmployeeCode(employeeCode));
    }
}