using HRemployee.Entities;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;

namespace HRemployee.Service.Interface
{
    public interface IService_Department
    {
        Task<ResponseObject<PagedResult<DTO_Department>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null);
        Task<ResponseObject<DTO_Department>> CreateAsync(Request_CreateDepartment request);
        Task<ResponseObject<DTO_Department>> UpdateAsync(int id, Request_UpdateDepartment request);
        Task<ResponseObject<DTO_Department>> UpdateByCodeAsync(string code, Request_UpdateDepartment request);
        Task<ResponseBase> DeleteAsync(int id);
        Task<ResponseBase> DeleteByCodeAsync(string code);
    }
}