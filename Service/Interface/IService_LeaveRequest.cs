using HRemployee.Entities;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;

namespace HRemployee.Service.Interface
{
    public interface IService_LeaveRequest
    {
        Task<ResponseObject<DTO_LeaveRequest>> CreateAsync(int employeeId, Request_CreateLeaveRequest request);

        Task<ResponseObject<List<DTO_LeaveRequest>>> GetMyAsync(int employeeId, string? status = null);

        Task<ResponseObject<List<DTO_LeaveRequest>>> GetPendingAsync(int approverId);

        Task<ResponseBase> ApproveAsync(int leaveRequestId, int approverId);

        Task<ResponseBase> RejectAsync(int leaveRequestId, int approverId, Request_RejectLeaveRequest request);

        Task<ResponseObject<PagedResult<DTO_LeaveRequest>>> GetAllAsync(int managerId, string role, int pageNumber = 1, int pageSize = 10, string? search = null);
    }
}