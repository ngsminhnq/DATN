using HRemployee.DataContext;
using HRemployee.Entities;
using HRemployee.Enums;
using HRemployee.PayLoad.Converter;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;
using HRemployee.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace HRemployee.Service.Implement
{
    public class Service_Attendance : IService_Attendance
    {
        private readonly AppDbContext _db;
        private readonly Converter_Attendance _converter;
        private readonly ResponseBase _responseBase;
        private readonly ResponseObject<DTO_Attendance> _responseObj;
        private readonly ResponseObject<List<DTO_Attendance>> _responseList;
        private readonly ResponseObject<DTO_AttendanceSummary> _responseSummary;

        private static readonly TimeSpan CHECKIN_DEADLINE = new TimeSpan(8, 30, 0);
        private const int VN_UTC_OFFSET = 7;

        private const decimal HALF_DAY_THRESHOLD = 4m;
        private const decimal FULL_DAY_THRESHOLD = 7m;

        public Service_Attendance(
            AppDbContext db,
            Converter_Attendance converter,
            ResponseBase responseBase,
            ResponseObject<DTO_Attendance> responseObj,
            ResponseObject<List<DTO_Attendance>> responseList,
            ResponseObject<DTO_AttendanceSummary> responseSummary)
        {
            _db = db;
            _converter = converter;
            _responseBase = responseBase;
            _responseObj = responseObj;
            _responseList = responseList;
            _responseSummary = responseSummary;
        }

        public async Task<ResponseObject<DTO_Attendance>> CheckInAsync(int employeeId, Request_CheckIn request)
        {
            var employee = await _db.Employees.Include(e => e.Contracts).FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted);

            if (employee == null) return _responseObj.ResponseObjectError(404, "Nhân viên không tồn tại", null);

            var today = DateTime.UtcNow.Date;
            if (!employee.Contracts.Any(c => c.Status == ContractStatusEnum.Active && c.EndDate >= today)) return _responseObj.ResponseObjectError(400, "Nhân viên không có hợp đồng đang hiệu lực hoặc hợp đồng đã hết hạn", null);

            var existing = await _db.Attendances.FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);

            if (existing != null && existing.CheckInTime.HasValue) return _responseObj.ResponseObjectError(400, "Bạn đã check-in hôm nay rồi!", null);

            var now = DateTime.UtcNow;

            var vnTime = now.AddHours(VN_UTC_OFFSET);
            var status = vnTime.TimeOfDay > CHECKIN_DEADLINE ? AttendanceStatusEnum.Late : AttendanceStatusEnum.Present;

            Attendance attendance;
            if (existing != null)
            {
                existing.CheckInTime = now;
                existing.Status = status;
                existing.Note = request.Note;
                attendance = existing;
            }
            else
            {
                attendance = new Attendance
                {
                    EmployeeId = employeeId, Date = today, CheckInTime = now, Status = status, Note = request.Note, CreatedAt = DateTime.UtcNow };
                await _db.Attendances.AddAsync(attendance);
            }

            await _db.SaveChangesAsync();

            var saved = await _db.Attendances.Include(a => a.Employee).FirstOrDefaultAsync(a => a.Id == attendance.Id);

            return _responseObj.ResponseObjectSuccess( status == AttendanceStatusEnum.Late ? "Check-in thành công! (Lưu ý: Bạn đến muộn)" : "Check-in thành công!", _converter.ToDTO(saved));
        }

        public async Task<ResponseObject<DTO_Attendance>> CheckOutAsync(int employeeId, Request_CheckOut request)
        {
            var today = DateTime.UtcNow.Date;
            var attendance = await _db.Attendances.Include(a => a.Employee).FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);

            if (attendance == null || !attendance.CheckInTime.HasValue) return _responseObj.ResponseObjectError(400, "Bạn chưa check-in hôm nay!", null);

            if (attendance.CheckOutTime.HasValue)
                return _responseObj.ResponseObjectError(400, "Bạn đã check-out hôm nay rồi!", null);

            var now = DateTime.UtcNow;
            attendance.CheckOutTime = now;

            var workingHours = (decimal)(now - attendance.CheckInTime.Value).TotalHours;
            attendance.WorkingHours = Math.Round(workingHours, 2);

            attendance.WorkingDays = workingHours switch
            {
                < HALF_DAY_THRESHOLD => 0m, >= HALF_DAY_THRESHOLD and < FULL_DAY_THRESHOLD => 0.5m, _ => 1.0m };

            attendance.Note = string.IsNullOrEmpty(request.Note) ? attendance.Note : $"{attendance.Note}; {request.Note}";

            await _db.SaveChangesAsync();

            return _responseObj.ResponseObjectSuccess( $"Check-out thành công! Giờ làm: {attendance.WorkingHours}h ({attendance.WorkingDays} ngày công)", _converter.ToDTO(attendance));
        }

        public async Task<ResponseObject<DTO_Attendance>> GetTodayStatusAsync(int employeeId)
        {
            var today = DateTime.UtcNow.Date;
            var attendance = await _db.Attendances.Include(a => a.Employee).FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == today);

            if (attendance == null) return _responseObj.ResponseObjectError(200, "Hôm nay chưa check-in", null);

            return _responseObj.ResponseObjectSuccess("Thành công", _converter.ToDTO(attendance));
        }

        public async Task<ResponseObject<List<DTO_Attendance>>> GetByEmployeeAsync(int employeeId, int month, int year)
        {
            var attendances = await _db.Attendances.Include(a => a.Employee).Where(a => a.EmployeeId == employeeId && a.Date.Month == month && a.Date.Year == year).OrderBy(a => a.Date).ToListAsync();

            return _responseList.ResponseObjectSuccess("Thành công", attendances.Select(a => _converter.ToDTO(a)).ToList());
        }

        public async Task<ResponseObject<DTO_AttendanceSummary>> GetSummaryAsync(int employeeId, int month, int year)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted);
            if (employee == null) return _responseSummary.ResponseObjectError(404, "Nhân viên không tồn tại", null);

            var attendances = await _db.Attendances.Where(a => a.EmployeeId == employeeId && a.Date.Month == month && a.Date.Year == year).ToListAsync();

            var summary = new DTO_AttendanceSummary
            {
                EmployeeId = employeeId, EmployeeName = employee.FullName, Month = month, Year = year, TotalWorkingDays = attendances.Sum(a => a.WorkingDays ?? 0), PresentDays = attendances.Count(a => a.Status == AttendanceStatusEnum.Present), LateDays = attendances.Count(a => a.Status == AttendanceStatusEnum.Late), AbsentDays = attendances.Count(a => a.Status == AttendanceStatusEnum.Absent), LeaveDays = attendances.Count(a => a.Status == AttendanceStatusEnum.Leave), TotalWorkingHours = attendances.Sum(a => a.WorkingHours ?? 0) };

            return _responseSummary.ResponseObjectSuccess("Thống kê thành công", summary);
        }

        public async Task<ResponseObject<DTO_Attendance>> EditAsync(int attendanceId, Request_EditAttendance request)
        {
            var attendance = await _db.Attendances.Include(a => a.Employee).FirstOrDefaultAsync(a => a.Id == attendanceId);

            if (attendance == null) return _responseObj.ResponseObjectError(404, "Không tìm thấy bản ghi chấm công!", null);

            if (request.CheckInTime.HasValue)
                attendance.CheckInTime = request.CheckInTime.Value;

            if (request.CheckOutTime.HasValue)
                attendance.CheckOutTime = request.CheckOutTime.Value;

            if (attendance.CheckInTime.HasValue && attendance.CheckOutTime.HasValue)
            {
                var hours = (decimal)(attendance.CheckOutTime.Value - attendance.CheckInTime.Value).TotalHours;

                if (hours < 0)
                    return _responseObj.ResponseObjectError(400, "Giờ check-out không được nhỏ hơn giờ check-in!", null);

                attendance.WorkingHours = Math.Round(hours, 2);

                attendance.WorkingDays = hours switch
                {
                    < HALF_DAY_THRESHOLD => 0m, >= HALF_DAY_THRESHOLD and < FULL_DAY_THRESHOLD => 0.5m, _ => 1.0m };
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                if (Enum.TryParse<AttendanceStatusEnum>(request.Status, out var status))
                    attendance.Status = status;
                else
                    return _responseObj.ResponseObjectError(400, "Trạng thái không hợp lệ! Dùng: Present / Late / Absent / Leave", null);
            }
            else if (attendance.CheckInTime.HasValue && attendance.WorkingDays > 0)
            {
                attendance.Status = attendance.CheckInTime.Value.TimeOfDay > CHECKIN_DEADLINE ? AttendanceStatusEnum.Late : AttendanceStatusEnum.Present;
            }

            if (!string.IsNullOrEmpty(request.Note))
                attendance.Note = request.Note;

            await _db.SaveChangesAsync();

            return _responseObj.ResponseObjectSuccess("Sửa chấm công thành công!", _converter.ToDTO(attendance));
        }

        public async Task<ResponseBase> CalculateMonthlyAsync(int employeeId, int month, int year)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted);

            if (employee == null) return _responseBase.ResponseBaseError(404, "Nhân viên không tồn tại!");

            if (month < 1 || month > 12)
                return _responseBase.ResponseBaseError(400, "Tháng không hợp lệ (1-12)!");

            var existingDates = await _db.Attendances.Where(a => a.EmployeeId == employeeId && a.Date.Month == month && a.Date.Year == year).Select(a => a.Date.Date).ToListAsync();

            var daysInMonth = DateTime.DaysInMonth(year, month);
            var toInsert = new List<Attendance>();
            var today = DateTime.UtcNow.Date;

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);

                if (date.Date > today) continue;
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;
                if (existingDates.Contains(date.Date)) continue;

                toInsert.Add(new Attendance
                {
                    EmployeeId = employeeId, Date = date, Status = AttendanceStatusEnum.Absent, WorkingHours = 0, WorkingDays = 0, Note = "Tự điền (Không chấm công)", CreatedAt = DateTime.UtcNow });
            }

            if (toInsert.Any())
            {
                await _db.Attendances.AddRangeAsync(toInsert);
                await _db.SaveChangesAsync();
            }

            return _responseBase.ResponseBaseSuccess($"Tổng hợp tháng {month}/{year} xong! Đã thêm {toInsert.Count} ngày Absent.");
        }

        private async Task<int> GetEmployeeIdByCode(string employeeCode)
        {
            var emp = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode.ToUpper() && !e.IsDeleted);
            return emp?.Id ?? 0;
        }

        public async Task<ResponseObject<List<DTO_Attendance>>> GetByEmployeeByCodeAsync(string employeeCode, int month, int year)
        {
            int id = await GetEmployeeIdByCode(employeeCode);
            if (id == 0) return _responseList.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);

            return await GetByEmployeeAsync(id, month, year);
        }

        public async Task<ResponseObject<DTO_AttendanceSummary>> GetSummaryByCodeAsync(string employeeCode, int month, int year)
        {
            int id = await GetEmployeeIdByCode(employeeCode);
            if (id == 0) return _responseSummary.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);

            return await GetSummaryAsync(id, month, year);
        }

        public async Task<ResponseBase> CalculateMonthlyByCodeAsync(string employeeCode, int month, int year)
        {
            int id = await GetEmployeeIdByCode(employeeCode);
            if (id == 0) return _responseBase.ResponseBaseError(404, $"Không tìm thấy nhân viên '{employeeCode}'");

            return await CalculateMonthlyAsync(id, month, year);
        }

        public async Task<ResponseObject<DTO_Attendance>> EditByCodeDateAsync(string employeeCode, string date, Request_EditAttendance request)
        {
            int empId = await GetEmployeeIdByCode(employeeCode);
            if (empId == 0) return _responseObj.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);

            if (!DateTime.TryParseExact(date, new[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedDate))
                return _responseObj.ResponseObjectError(400, "Định dạng ngày không hợp lệ. Dùng: dd/MM/yyyy (vd: 12/05/2026)", null);

            var dateUtc = DateTime.SpecifyKind(parsedDate.Date, DateTimeKind.Utc);
            var attendance = await _db.Attendances.Include(a => a.Employee).FirstOrDefaultAsync(a => a.EmployeeId == empId && a.Date == dateUtc);

            if (attendance == null)
            {
                attendance = new Attendance
                {
                    EmployeeId = empId, Date = dateUtc, Status = AttendanceStatusEnum.Absent, WorkingHours = 0, WorkingDays = 0, Note = "Tự động khởi tạo khi bổ sung chấm công", CreatedAt = DateTime.UtcNow };
                await _db.Attendances.AddAsync(attendance);
                await _db.SaveChangesAsync();
            }

            return await EditAsync(attendance.Id, request);
        }
    }
}