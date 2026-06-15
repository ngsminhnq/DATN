using HRemployee.DataContext;
using HRemployee.Entities;
using HRemployee.Enums;
using HRemployee.Helper;
using HRemployee.PayLoad.Converter;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;
using HRemployee.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace HRemployee.Service.Implement
{
    public class Service_SalaryRecord : IService_SalaryRecord
    {
        private readonly AppDbContext _db;
        private readonly Converter_SalaryRecord _converter;
        private readonly ResponseBase _responseBase;
        private readonly ResponseObject<DTO_SalaryRecord> _responseObj;
        private readonly ResponseObject<List<DTO_SalaryRecord>> _responseList;
        private readonly ResponseObject<PagedResult<DTO_SalaryRecord>> _responsePagedList;

        public Service_SalaryRecord(AppDbContext db, Converter_SalaryRecord converter, ResponseBase responseBase, ResponseObject<DTO_SalaryRecord> responseObj, ResponseObject<List<DTO_SalaryRecord>> responseList, ResponseObject<PagedResult<DTO_SalaryRecord>> responsePagedList)
        {
            _db           = db;
            _converter    = converter;
            _responseBase = responseBase;
            _responseObj  = responseObj;
            _responseList = responseList;
            _responsePagedList = responsePagedList;
        }

        private static int GetStandardDays(int month, int year)
        {
            int days = 0;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            for (int d = 1; d <= daysInMonth; d++)
            {
                var dow = new DateTime(year, month, d).DayOfWeek;
                if (dow != DayOfWeek.Saturday && dow != DayOfWeek.Sunday) days++;
            }
            return days;
        }

        private async Task<decimal> GetActualWorkingDays(int employeeId, int month, int year)
            => await _db.Attendances.Where(a => a.EmployeeId == employeeId && a.Date.Month == month && a.Date.Year == year && (a.Status == AttendanceStatusEnum.Present || a.Status == AttendanceStatusEnum.Late || a.Status == AttendanceStatusEnum.Leave)).SumAsync(a => a.WorkingDays ?? 0m);

        private async Task<ResponseObject<DTO_SalaryRecord>> ComputeSalary(Employee employee, int month, int year, Request_CalculateSalary request)
        {
            var config = await _db.SalaryConfigs.Where(s => s.EmployeeId == employee.Id && s.IsActive).FirstOrDefaultAsync();
            if (config == null) return _responseObj.ResponseObjectError(400, $"Nhân viên '{employee.EmployeeCode}' chưa có cấu hình lương. Tạo SalaryConfig trước.", null);

            var contract = await _db.Contracts.Where(c => c.EmployeeId == employee.Id && c.Status == ContractStatusEnum.Active).FirstOrDefaultAsync();
            if (contract == null) return _responseObj.ResponseObjectError(400, $"Nhân viên '{employee.EmployeeCode}' không có hợp đồng đang hiệu lực.", null);

            int standardDays = GetStandardDays(month, year);
            decimal workingDays = await GetActualWorkingDays(employee.Id, month, year);
            if (standardDays == 0) return _responseObj.ResponseObjectError(400, "Tháng không hợp lệ (không có ngày làm việc).", null);

            decimal ratio         = workingDays / standardDays;
            decimal salaryPercent = contract.SalaryPercent;
            decimal luongTheoNgay = config.BaseSalary * (salaryPercent / 100m) * ratio;
            decimal phuCap        = config.Allowance * ratio;
            decimal kpiCoeff      = Math.Clamp(request.KpiCoefficient, 0m, 1.5m);
            decimal thuongKpi     = config.KpiBonus * kpiCoeff * ratio;
            decimal grossSalary   = luongTheoNgay + phuCap + thuongKpi;

            decimal totalBH = config.BaseSalary * 0.105m;

            decimal thuNhapChiuThue = grossSalary - totalBH - 11_000_000m;
            decimal tax             = thuNhapChiuThue > 0 ? thuNhapChiuThue * 0.10m : 0m;
            decimal netSalary       = grossSalary - totalBH - tax;

            var existing = await _db.SalaryRecords.FirstOrDefaultAsync(s => s.EmployeeId == employee.Id && s.Month == month && s.Year == year);
            if (existing != null) _db.SalaryRecords.Remove(existing);

            var record = new SalaryRecord
            {
                EmployeeId = employee.Id, Month = month, Year = year, BaseSalary = config.BaseSalary, Allowance = config.Allowance, KpiBonus = config.KpiBonus, KpiCoefficient = kpiCoeff, SalaryPercent = (int)salaryPercent, StandardDays = standardDays, WorkingDays = workingDays, GrossSalary = Math.Round(grossSalary, 0), InsuranceDeduction = Math.Round(totalBH, 0), TaxDeduction = Math.Round(tax, 0), NetSalary = Math.Round(netSalary, 0), Status = SalaryStatusEnum.Pending, Note = request.Note ?? $"Lương tháng {month}/{year}", CreatedAt = DateTime.UtcNow };

            await _db.SalaryRecords.AddAsync(record);
            await _db.SaveChangesAsync();

            var saved = await _db.SalaryRecords.Include(s => s.Employee).FirstOrDefaultAsync(s => s.Id == record.Id);
            return _responseObj.ResponseObjectSuccess($"Tính lương tháng {month}/{year} cho {employee.EmployeeCode} thành công!", _converter.ToDTO(saved!));
        }

        public async Task<ResponseObject<DTO_SalaryRecord>> CalculateAsync(string employeeCode, int month, int year, Request_CalculateSalary request)
        {
            if (month < 1 || month > 12)
                return _responseObj.ResponseObjectError(400, "Tháng không hợp lệ (1-12)", null);

            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode.ToUpper() && !e.IsDeleted);
            if (employee == null) return _responseObj.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);

            return await ComputeSalary(employee, month, year, request);
        }

        public async Task<ResponseBase> CalculateAllAsync(int month, int year, decimal kpiCoefficient = 1.0m)
        {
            if (month < 1 || month > 12)
                return _responseBase.ResponseBaseError(400, "Tháng không hợp lệ (1-12)");

            var employees = await _db.Employees.Where(e => !e.IsDeleted && e.Status == EmployeeStatusEnum.Active).ToListAsync();
            int success = 0, fail = 0;
            var request = new Request_CalculateSalary { KpiCoefficient = kpiCoefficient, Note = $"Lương tháng {month}/{year} (hàng loạt)" };

            foreach (var emp in employees)
            {
                var result = await ComputeSalary(emp, month, year, request);
                if (result.Status == 200) success++; else fail++;
            }

            return _responseBase.ResponseBaseSuccess($"Tính lương tháng {month}/{year} hoàn tất! Thành công: {success}, Bỏ qua: {fail}");
        }

        public async Task<ResponseObject<DTO_SalaryRecord>> GetMyAsync(int employeeId, int month, int year)
        {
            var record = await _db.SalaryRecords.Include(s => s.Employee).FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.Month == month && s.Year == year);
            if (record == null) return _responseObj.ResponseObjectError(404, $"Chưa có phiếu lương tháng {month}/{year}. Liên hệ kế toán để tính lương.", null);

            return _responseObj.ResponseObjectSuccess("Thành công", _converter.ToDTO(record));
        }

        public async Task<ResponseObject<List<DTO_SalaryRecord>>> GetByEmployeeAsync(string employeeCode)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode.ToUpper() && !e.IsDeleted);
            if (employee == null) return _responseList.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);

            var records = await _db.SalaryRecords.Include(s => s.Employee).Where(s => s.EmployeeId == employee.Id).OrderByDescending(s => s.Year).ThenByDescending(s => s.Month).ToListAsync();
            return _responseList.ResponseObjectSuccess($"Lịch sử phiếu lương của {employeeCode}: {records.Count} tháng", records.Select(s => _converter.ToDTO(s)).ToList());
        }

        public async Task<ResponseObject<PagedResult<DTO_SalaryRecord>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, int? month = null, int? year = null)
        {
            var query = _db.SalaryRecords.Include(s => s.Employee).AsQueryable();

            if (month.HasValue) query = query.Where(s => s.Month == month.Value);
            if (year.HasValue)  query = query.Where(s => s.Year  == year.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(s => s.Employee.EmployeeCode.Contains(search) || s.Employee.FullName.Contains(search));

            int totalItems = await query.CountAsync();
            var items = await query.OrderByDescending(s => s.Year).ThenByDescending(s => s.Month).ThenBy(s => s.Employee.EmployeeCode).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = new PagedResult<DTO_SalaryRecord>
            {
                Items = items.Select(s => _converter.ToDTO(s)).ToList(), TotalItems = totalItems, TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize), CurrentPage = pageNumber };

            return _responsePagedList.ResponseObjectSuccess("Lấy danh sách phiếu lương thành công", result);
        }

        public async Task<ResponseBase> SendPayslipEmailAsync(string employeeCode, int month, int year)
        {
            var record = await _db.SalaryRecords.Include(s => s.Employee).FirstOrDefaultAsync(s => s.Employee.EmployeeCode == employeeCode.ToUpper() && s.Month == month && s.Year == year);
            if (record == null) return _responseBase.ResponseBaseError(404, $"Chưa có phiếu lương tháng {month}/{year} của {employeeCode}. Tính lương trước.");

            var email = record.Employee.Email;
            if (string.IsNullOrEmpty(email))
                return _responseBase.ResponseBaseError(400, "Nhân viên không có email.");

            var emailSender = new EmailTo();
            var result = await emailSender.SendEmailAsync(new EmailTo
            {
                Mail = email, Subject = $"[HR System] Phiếu lương tháng {month}/{year} - {record.Employee.FullName}", Content = BuildPayslipHtml(record) });

            record.Status = SalaryStatusEnum.Paid;
            await _db.SaveChangesAsync();

            return _responseBase.ResponseBaseSuccess($"Đã gửi phiếu lương tháng {month}/{year} đến {email}. {result}");
        }

        private static string BuildPayslipHtml(SalaryRecord r)
        {
            string contractType = r.SalaryPercent switch
            {
                70 => "Học việc (70%)", 85 => "Thử việc (85%)", 100 => "Chính thức (100%)", _ => $"{r.SalaryPercent}%" };

            return $@"
<!DOCTYPE html>
<html lang='vi'>
<head><meta charset='UTF-8'>
<style>
  body {{ font-family: Arial, sans-serif; background: #f5f5f5; padding: 20px; }}.container {{ max-width: 650px; margin: auto; background: #fff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,.1); }}.header {{ background: #1565C0; color: #fff; padding: 24px; text-align: center; }}.header h2 {{ margin: 0; font-size: 22px; }}.header p {{ margin: 4px 0 0; opacity: .85; }}.section {{ padding: 20px 28px; border-bottom: 1px solid #eee; }}.section h3 {{ margin: 0 0 12px; color: #1565C0; font-size: 14px; text-transform: uppercase; letter-spacing: .5px; }}
  table {{ width: 100%; border-collapse: collapse; }}
  td {{ padding: 7px 0; font-size: 14px; color: #333; }}
  td:last-child {{ text-align: right; font-weight: 600; }}.highlight td {{ background: #E3F2FD; }}.total td {{ background: #1565C0; color: #fff; font-size: 16px; padding: 12px 8px; border-radius: 4px; }}.footer {{ padding: 16px 28px; font-size: 12px; color: #888; text-align: center; }}
</style>
</head>
<body>
<div class='container'> <div class='header'> <h2>PHIẾU LƯƠNG THÁNG {r.Month}/{r.Year}</h2> <p>{r.Employee?.FullName} - {r.Employee?.EmployeeCode}</p> </div> <div class='section'> <h3>Thông tin ngày công</h3> <table> <tr><td>Loại hợp đồng</td><td>{contractType}</td></tr> <tr><td>Ngày công chuẩn (T2-T6)</td><td>{r.StandardDays} ngày</td></tr> <tr><td>Ngày công thực tế</td><td>{r.WorkingDays} ngày</td></tr> </table> </div> <div class='section'> <h3>Các khoản thu nhập</h3> <table> <tr><td>Lương cơ bản</td><td>{r.BaseSalary:N0} đ</td></tr> <tr><td>Phụ cấp</td><td>{r.Allowance:N0} đ</td></tr> <tr><td>Thưởng KPI (×{r.KpiCoefficient})</td><td>{r.KpiBonus * r.KpiCoefficient:N0} đ</td></tr> <tr class='highlight'><td><b>Tổng lương gộp</b></td><td><b>{r.GrossSalary:N0} đ</b></td></tr> </table> </div> <div class='section'> <h3>Các khoản khấu trừ</h3> <table> <tr><td>Bảo hiểm (BHXH 8% + BHYT 1.5% + BHTN 1%)</td><td>- {r.InsuranceDeduction:N0} đ</td></tr> <tr><td>Thuế TNCN</td><td>- {r.TaxDeduction:N0} đ</td></tr> </table> </div> <div class='section'> <table> <tr class='total'><td>LƯƠNG THỰC NHẬN</td><td>{r.NetSalary:N0} đ</td></tr> </table> </div> <div class='footer'> Phiếu lương được tạo tự động bởi HR Management System.<br> Vui lòng liên hệ phòng nhân sự nếu có thắc mắc. </div> </div> </body> </html>";
        }
    }
}