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
    public class Controller_LeaveRequest : ControllerBase
    {
        private readonly IService_LeaveRequest _service;

        public Controller_LeaveRequest(IService_LeaveRequest service)
        {
            _service = service;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Request_CreateLeaveRequest request)
        {
            int employeeId = GetEmployeeId();
            if (employeeId == 0) return Unauthorized("Không xác định được nhân viên từ token!");
            return Ok(await _service.CreateAsync(employeeId, request));
        }

        [HttpGet("MyRequests")]
        public async Task<IActionResult> MyRequests()
        {
            int employeeId = GetEmployeeId();
            if (employeeId == 0) return Unauthorized("Không xác định được nhân viên từ token!");
            return Ok(await _service.GetMyAsync(employeeId, null));
        }

        [HttpGet("Pending")]
        [Authorize(Roles = "Director,BlockManager,CenterManager")]
        public async Task<IActionResult> Pending()
        {
            int approverId = GetEmployeeId();
            if (approverId == 0) return Unauthorized("Không xác định được nhân viên từ token!");
            return Ok(await _service.GetPendingAsync(approverId));
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Director,BlockManager,CenterManager")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            int managerId = GetEmployeeId();
            if (managerId == 0) return Unauthorized("Không xác định được nhân viên từ token!");
            var role = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value ?? "";
            return Ok(await _service.GetAllAsync(managerId, role, pageNumber, pageSize, search));
        }

        [HttpPut("Approve/{leaveRequestId}")]
        [Authorize(Roles = "Director,BlockManager,CenterManager")]
        public async Task<IActionResult> Approve(int leaveRequestId)
        {
            int approverId = GetEmployeeId();
            if (approverId == 0) return Unauthorized("Không xác định được nhân viên từ token!");
            return Ok(await _service.ApproveAsync(leaveRequestId, approverId));
        }

        [HttpPut("Reject/{leaveRequestId}")]
        [Authorize(Roles = "Director,BlockManager,CenterManager")]
        public async Task<IActionResult> Reject(int leaveRequestId, [FromBody] Request_RejectLeaveRequest request)
        {
            int approverId = GetEmployeeId();
            if (approverId == 0) return Unauthorized("Không xác định được nhân viên từ token!");
            return Ok(await _service.RejectAsync(leaveRequestId, approverId, request));
        }

        private int GetEmployeeId()
        {
            var claim = HttpContext.User.FindFirst("EmployeeId");
            return int.TryParse(claim?.Value, out int id) ? id : 0;
        }
    }
}