using HRemployee.Entities;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;

namespace HRemployee.Service.Interface
{
    public interface IService_SalaryRecord
    {

        Task<ResponseObject<DTO_SalaryRecord>> CalculateAsync(string employeeCode, int month, int year, Request_CalculateSalary request);

        Task<ResponseBase> CalculateAllAsync(int month, int year, decimal kpiCoefficient = 1.0m);

        Task<ResponseObject<DTO_SalaryRecord>> GetMyAsync(int employeeId, int month, int year);

        Task<ResponseObject<List<DTO_SalaryRecord>>> GetByEmployeeAsync(string employeeCode);

        Task<ResponseObject<PagedResult<DTO_SalaryRecord>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, int? month = null, int? year = null);

        Task<ResponseBase> SendPayslipEmailAsync(string employeeCode, int month, int year);
    }
}