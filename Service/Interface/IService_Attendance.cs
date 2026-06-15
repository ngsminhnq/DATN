using HRemployee.Entities;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;

namespace HRemployee.Service.Interface
{
    public interface IService_Attendance
    {
        Task<ResponseObject<DTO_Attendance>> CheckInAsync(int employeeId, Request_CheckIn request);

        Task<ResponseObject<DTO_Attendance>> CheckOutAsync(int employeeId, Request_CheckOut request);

        Task<ResponseObject<DTO_Attendance>> GetTodayStatusAsync(int employeeId);

        Task<ResponseObject<List<DTO_Attendance>>> GetByEmployeeAsync(int employeeId, int month, int year);
        Task<ResponseObject<List<DTO_Attendance>>> GetByEmployeeByCodeAsync(string employeeCode, int month, int year);

        Task<ResponseObject<DTO_AttendanceSummary>> GetSummaryAsync(int employeeId, int month, int year);
        Task<ResponseObject<DTO_AttendanceSummary>> GetSummaryByCodeAsync(string employeeCode, int month, int year);

        Task<ResponseObject<DTO_Attendance>> EditAsync(int attendanceId, Request_EditAttendance request);
        Task<ResponseObject<DTO_Attendance>> EditByCodeDateAsync(string employeeCode, string date, Request_EditAttendance request);

        Task<ResponseBase> CalculateMonthlyAsync(int employeeId, int month, int year);
        Task<ResponseBase> CalculateMonthlyByCodeAsync(string employeeCode, int month, int year);
    }
}