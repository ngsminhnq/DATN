using HRemployee.Entities;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;

namespace HRemployee.Service.Interface
{
    public interface IService_LeaveType
    {
        Task<ResponseObject<List<DTO_LeaveType>>> GetAllAsync(string? search = null);
        Task<ResponseObject<DTO_LeaveType>> CreateAsync(Request_CreateLeaveType request);
        Task<ResponseObject<DTO_LeaveType>> UpdateAsync(int id, Request_UpdateLeaveType request);
        Task<ResponseBase> DeleteAsync(int id);
    }
}