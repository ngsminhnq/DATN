using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Response;

namespace HRemployee.Service.Interface
{
    public interface IService_Statistics
    {

        Task<ResponseObject<DTO_StatEmployee>> GetEmployeeStatsAsync();

        Task<ResponseObject<DTO_StatAttendance>> GetAttendanceStatsAsync(int month, int year);

        Task<ResponseObject<DTO_StatSalary>> GetSalaryStatsAsync(int month, int year);
    }
}