using HRemployee.Entities;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;

namespace HRemployee.Service.Interface
{
    public interface IService_Contract
    {
        Task<ResponseObject<PagedResult<DTO_Contract>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null);
        Task<ResponseObject<List<DTO_Contract>>> GetByEmployeeAsync(int employeeId);
        Task<ResponseObject<List<DTO_Contract>>> GetByEmployeeByCodeAsync(string employeeCode);

        Task<ResponseObject<DTO_Contract>> GetCurrentAsync(int employeeId);
        Task<ResponseObject<DTO_Contract>> GetCurrentByCodeAsync(string employeeCode);

        Task<ResponseObject<DTO_Contract>> CreateAsync(Request_CreateContract request);

        Task<ResponseObject<DTO_Contract>> RenewAsync(int contractId, Request_RenewContract request);
        Task<ResponseObject<DTO_Contract>> RenewByCodeAsync(string contractCode, Request_RenewContract request);

        Task<ResponseBase> TerminateAsync(int contractId, Request_TerminateContract request);
        Task<ResponseBase> TerminateByCodeAsync(string contractCode, Request_TerminateContract request);
    }
}