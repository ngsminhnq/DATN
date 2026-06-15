using HRemployee.Entities;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;

namespace HRemployee.Service.Interface
{
    public interface IService_Position
    {
        Task<ResponseObject<PagedResult<DTO_Position>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null);
        Task<ResponseObject<DTO_Position>> CreateAsync(Request_CreatePosition request);
        Task<ResponseObject<DTO_Position>> UpdateAsync(int id, Request_UpdatePosition request);
        Task<ResponseObject<DTO_Position>> UpdateByCodeAsync(string code, Request_UpdatePosition request);
        Task<ResponseBase> DeleteAsync(int id);
        Task<ResponseBase> DeleteByCodeAsync(string code);
    }
}