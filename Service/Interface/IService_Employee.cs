using HRemployee.Entities;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;
using Microsoft.AspNetCore.Http;

namespace HRemployee.Service.Interface
{
    public interface IService_Employee
    {
        Task<ResponseObject<PagedResult<DTO_Employee>>> GetAllAsync(
            int pageNumber, int pageSize,
            string? search = null);

        Task<ResponseObject<DTO_Employee>> GetByIdAsync(int id);
        Task<ResponseObject<DTO_Employee>> GetByCodeAsync(string employeeCode);

        Task<ResponseObject<DTO_Employee>> CreateAsync(Request_CreateEmployee request);

        Task<ResponseObject<DTO_Employee>> UpdateAsync(int id, Request_UpdateEmployee request);
        Task<ResponseObject<DTO_Employee>> UpdateByCodeAsync(string employeeCode, Request_UpdateEmployee request);

        Task<ResponseObject<DTO_Employee>> UploadAvatarAsync(int id, IFormFile avatar);
        Task<ResponseObject<DTO_Employee>> UploadAvatarByCodeAsync(string employeeCode, IFormFile avatar);

        Task<ResponseBase> DeleteAsync(int id);
        Task<ResponseBase> DeleteByCodeAsync(string employeeCode);
    }
}