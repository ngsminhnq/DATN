using HRemployee.Entities;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;

namespace HRemployee.Service.Interface
{
    public interface IService_SalaryConfig
    {

        Task<ResponseObject<PagedResult<DTO_SalaryConfig>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null);

        Task<ResponseObject<DTO_SalaryConfig>> CreateAsync(string employeeCode, Request_SalaryConfig request);

        Task<ResponseObject<DTO_SalaryConfig>> UpdateAsync(string employeeCode, Request_SalaryConfig request);

        Task<ResponseObject<DTO_SalaryConfig>> GetCurrentAsync(string employeeCode);

        Task<ResponseObject<List<DTO_SalaryConfig>>> GetHistoryAsync(string employeeCode);
    }
}