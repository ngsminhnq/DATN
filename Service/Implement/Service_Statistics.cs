using HRemployee.DataContext;
using HRemployee.Enums;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Response;
using HRemployee.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace HRemployee.Service.Implement
{
    public class Service_Statistics : IService_Statistics
    {
        private readonly AppDbContext _db;
        private readonly ResponseObject<DTO_StatEmployee>   _responseEmp;
        private readonly ResponseObject<DTO_StatAttendance> _responseAtt;
        private readonly ResponseObject<DTO_StatSalary>     _responseSal;

        public Service_Statistics(AppDbContext db, ResponseObject<DTO_StatEmployee> responseEmp, ResponseObject<DTO_StatAttendance> responseAtt, ResponseObject<DTO_StatSalary> responseSal)
        {
            _db          = db;
            _responseEmp = responseEmp;
            _responseAtt = responseAtt;
            _responseSal = responseSal;
        }

        public async Task<ResponseObject<DTO_StatEmployee>> GetEmployeeStatsAsync()
        {
            var employees = await _db.Employees.Include(e => e.Department).Include(e => e.Contracts).Where(e => !e.IsDeleted).ToListAsync();
            var departments = await _db.Departments.Where(d => !d.IsDeleted).ToListAsync();

            var byDept = departments.Select(dept =>
            {
                var deptEmps = employees.Where(e => e.DepartmentId == dept.Id).ToList();
                return new DTO_StatDepartment
                {
                    DepartmentId = dept.Id, DepartmentName = dept.Name, DepartmentCode = dept.Code, TotalEmployees = deptEmps.Count, ActiveEmployees = deptEmps.Count(e => e.Status == EmployeeStatusEnum.Active), InactiveEmployees = deptEmps.Count(e => e.Status == EmployeeStatusEnum.Inactive), ContractActive = deptEmps.Count(e => e.Contracts.Any(c => c.Status == ContractStatusEnum.Active)) };
            }).OrderByDescending(d => d.TotalEmployees).ToList();

            var result = new DTO_StatEmployee
            {
                TotalEmployees = employees.Count, ActiveEmployees = employees.Count(e => e.Status == EmployeeStatusEnum.Active), InactiveEmployees = employees.Count(e => e.Status == EmployeeStatusEnum.Inactive), TotalContracts = employees.Sum(e => e.Contracts.Count), ActiveContracts = employees.Sum(e => e.Contracts.Count(c => c.Status == ContractStatusEnum.Active)), ByDepartment = byDept };

            return _responseEmp.ResponseObjectSuccess("Thống kê nhân sự thành công", result);
        }

        public async Task<ResponseObject<DTO_StatAttendance>> GetAttendanceStatsAsync(int month, int year)
        {
            if (month < 1 || month > 12)
                return _responseAtt.ResponseObjectError(400, "Tháng không hợp lệ (1-12)", null);

            int standardDays = 0;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            for (int d = 1; d <= daysInMonth; d++)
            {
                var dow = new DateTime(year, month, d).DayOfWeek;
                if (dow != DayOfWeek.Saturday && dow != DayOfWeek.Sunday) standardDays++;
            }

            var employees = await _db.Employees.Include(e => e.Attendances.Where(a => a.Date.Month == month && a.Date.Year == year)).Where(e => !e.IsDeleted && e.Status == EmployeeStatusEnum.Active).ToListAsync();

            var details = employees.Select(e =>
            {
                var atts = e.Attendances.ToList();
                return new DTO_StatAttendanceEmployee
                {
                    EmployeeCode = e.EmployeeCode, EmployeeName = e.FullName, PresentDays = atts.Count(a => a.Status == AttendanceStatusEnum.Present), LateDays = atts.Count(a => a.Status == AttendanceStatusEnum.Late), AbsentDays = atts.Count(a => a.Status == AttendanceStatusEnum.Absent), LeaveDays = atts.Count(a => a.Status == AttendanceStatusEnum.Leave), TotalWorkingDays = atts.Sum(a => a.WorkingDays ?? 0m), TotalWorkingHours = (decimal)atts.Sum(a => a.WorkingHours ?? 0m) };
            }).OrderBy(e => e.EmployeeCode).ToList();

            var result = new DTO_StatAttendance
            {
                Month = month, Year = year, StandardDays = standardDays, TotalEmployees = employees.Count, FullyPresentCount = details.Count(d => d.AbsentDays == 0 && d.LeaveDays == 0), HasAbsentCount = details.Count(d => d.AbsentDays > 0), Details = details };

            return _responseAtt.ResponseObjectSuccess($"Thống kê chấm công tháng {month}/{year}", result);
        }

        public async Task<ResponseObject<DTO_StatSalary>> GetSalaryStatsAsync(int month, int year)
        {
            if (month < 1 || month > 12)
                return _responseSal.ResponseObjectError(400, "Tháng không hợp lệ (1-12)", null);

            var records = await _db.SalaryRecords.Where(s => s.Month == month && s.Year == year).ToListAsync();

            if (!records.Any())
                return _responseSal.ResponseObjectError(404, $"Chưa có dữ liệu lương tháng {month}/{year}. Chạy CalculateAll trước.", null);

            var result = new DTO_StatSalary
            {
                Month = month, Year = year, TotalEmployees = records.Count, TotalGrossSalary = records.Sum(s => s.GrossSalary), TotalInsurance = records.Sum(s => s.InsuranceDeduction), TotalTax = records.Sum(s => s.TaxDeduction), TotalNetSalary = records.Sum(s => s.NetSalary), AverageNetSalary = records.Average(s => s.NetSalary), MaxNetSalary = records.Max(s => s.NetSalary), MinNetSalary = records.Min(s => s.NetSalary) };

            return _responseSal.ResponseObjectSuccess($"Thống kê quỹ lương tháng {month}/{year}: {records.Count} phiếu, tổng thực trả {result.TotalNetSalary:N0} đ", result);
        }
    }
}