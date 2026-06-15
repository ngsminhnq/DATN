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
    public class Service_LeaveRequest : IService_LeaveRequest
    {
        private readonly AppDbContext _db;
        private readonly Converter_LeaveRequest _converter;
        private readonly ResponseBase _responseBase;
        private readonly ResponseObject<DTO_LeaveRequest> _responseObj;
        private readonly ResponseObject<List<DTO_LeaveRequest>> _responseList;
        private readonly ResponseObject<PagedResult<DTO_LeaveRequest>> _responsePagedList;

        public Service_LeaveRequest(
            AppDbContext db,
            Converter_LeaveRequest converter,
            ResponseBase responseBase,
            ResponseObject<DTO_LeaveRequest> responseObj,
            ResponseObject<List<DTO_LeaveRequest>> responseList,
            ResponseObject<PagedResult<DTO_LeaveRequest>> responsePagedList)
        {
            _db = db;
            _converter = converter;
            _responseBase = responseBase;
            _responseObj = responseObj;
            _responseList = responseList;
            _responsePagedList = responsePagedList;
        }

        public async Task<ResponseObject<DTO_LeaveRequest>> CreateAsync(int employeeId, Request_CreateLeaveRequest request)
        {
            var employee = await _db.Employees.Include(e => e.Contracts).Include(e => e.Manager).FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted);

            if (employee == null) return _responseObj.ResponseObjectError(404, "Nhân viên không tồn tại!", null);

            if (!employee.Contracts.Any(c => c.Status == ContractStatusEnum.Active)) return _responseObj.ResponseObjectError(400, "Nhân viên không có hợp đồng đang hiệu lực!", null);

            var leaveType = await _db.LeaveTypes.FirstOrDefaultAsync(lt => lt.Id == request.LeaveTypeId);
            if (leaveType == null) return _responseObj.ResponseObjectError(404, "Loại nghỉ phép không tồn tại!", null);

            if (request.FromDate > request.ToDate)
                return _responseObj.ResponseObjectError(400, "Ngày bắt đầu không được sau ngày kết thúc!", null);

            if (request.FromDate.Date < DateTime.UtcNow.Date)
                return _responseObj.ResponseObjectError(400, "Không thể xin nghỉ ngày trong quá khứ!", null);

            var conflictExists = await _db.LeaveRequests.AnyAsync(lr => lr.EmployeeId == employeeId && lr.Status == LeaveStatusEnum.Approved && lr.FromDate <= request.ToDate && lr.ToDate >= request.FromDate);

            if (conflictExists)
                return _responseObj.ResponseObjectError(400, "Khoảng thời gian này trùng với đơn nghỉ đã được duyệt trước đó!", null);

            decimal totalDays;
            if (request.FromDate.Date == request.ToDate.Date) totalDays = 0.5m;
            else
            {
                int count = 0;
                var d = request.FromDate.Date;
                while (d <= request.ToDate.Date)
                {
                    if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday) count++;
                    d = d.AddDays(1);
                }
                totalDays = count;
            }

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = employeeId, LeaveTypeId = request.LeaveTypeId, FromDate = request.FromDate.ToUniversalTime(), ToDate = request.ToDate.ToUniversalTime(), TotalDays = totalDays, Reason = request.Reason, Status = LeaveStatusEnum.Pending, CreatedAt = DateTime.UtcNow };

            await _db.LeaveRequests.AddAsync(leaveRequest);
            await _db.SaveChangesAsync();

            if (employee.ManagerId.HasValue && employee.Manager != null)
            {
                var managerUser = await _db.Users.FirstOrDefaultAsync(u => u.EmployeeId == employee.ManagerId);

                if (managerUser != null)
                {
                    var emailTo = new EmailTo
                    {
                        Mail = managerUser.Email, Subject = "[HỆ THỐNG NHÂN SỰ] ĐƠN XIN NGHỈ PHÉP CẦN DUYỆT", Content = $@" <h3>Xin chào {employee.Manager.FullName},</h3> <p>Nhân viên <strong>{employee.FullName}</strong> ({employee.EmployeeCode}) vừa tạo đơn xin nghỉ phép cần bạn duyệt.</p> <table border='1' cellpadding='8'> <tr><td><b>Loại nghỉ</b></td><td>{leaveType.Name}</td></tr> <tr><td><b>Từ ngày</b></td><td>{request.FromDate:dd/MM/yyyy}</td></tr> <tr><td><b>Đến ngày</b></td><td>{request.ToDate:dd/MM/yyyy}</td></tr> <tr><td><b>Số ngày</b></td><td>{totalDays}</td></tr> <tr><td><b>Lý do</b></td><td>{request.Reason}</td></tr> </table> <p>Vui lòng đăng nhập hệ thống để duyệt hoặc từ chối đơn này.</p>" };
                    await emailTo.SendEmailAsync(emailTo);
                }
            }

            var saved = await _db.LeaveRequests.Include(lr => lr.Employee).Include(lr => lr.LeaveType).Include(lr => lr.ApprovedBy).FirstOrDefaultAsync(lr => lr.Id == leaveRequest.Id);

            return _responseObj.ResponseObjectSuccess("Tạo đơn nghỉ phép thành công! Đang chờ cấp trên duyệt.", _converter.ToDTO(saved));
        }

        public async Task<ResponseObject<List<DTO_LeaveRequest>>> GetMyAsync(int employeeId, string? status = null)
        {
            var query = _db.LeaveRequests.Include(lr => lr.Employee).Include(lr => lr.LeaveType).Include(lr => lr.ApprovedBy).Where(lr => lr.EmployeeId == employeeId);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatusEnum>(status, out var statusEnum))
                query = query.Where(lr => lr.Status == statusEnum);

            var list = await query.OrderByDescending(lr => lr.CreatedAt).ToListAsync();

            return _responseList.ResponseObjectSuccess("Thành công", list.Select(lr => _converter.ToDTO(lr)).ToList());
        }

        public async Task<ResponseObject<List<DTO_LeaveRequest>>> GetPendingAsync(int approverId)
        {
            var subordinateIds = await _db.Employees.Where(e => e.ManagerId == approverId && !e.IsDeleted).Select(e => e.Id).ToListAsync();

            var list = await _db.LeaveRequests.Include(lr => lr.Employee).Include(lr => lr.LeaveType).Include(lr => lr.ApprovedBy).Where(lr => subordinateIds.Contains(lr.EmployeeId) && lr.Status == LeaveStatusEnum.Pending).OrderBy(lr => lr.CreatedAt).ToListAsync();

            return _responseList.ResponseObjectSuccess($"Có {list.Count} đơn đang chờ duyệt", list.Select(lr => _converter.ToDTO(lr)).ToList());
        }

        public async Task<ResponseBase> ApproveAsync(int leaveRequestId, int approverId)
        {
            var leaveRequest = await _db.LeaveRequests.Include(lr => lr.Employee).Include(lr => lr.LeaveType).FirstOrDefaultAsync(lr => lr.Id == leaveRequestId);

            if (leaveRequest == null) return _responseBase.ResponseBaseError(404, "Đơn nghỉ phép không tồn tại!");

            if (leaveRequest.Status != LeaveStatusEnum.Pending) return _responseBase.ResponseBaseError(400, $"Đơn này đã được xử lý (Trạng thái: {leaveRequest.Status})!");

            if (leaveRequest.Employee.ManagerId != approverId) return _responseBase.ResponseBaseError(403, "Bạn không có quyền duyệt đơn này (chỉ cấp trên trực tiếp mới được duyệt)!");

            leaveRequest.Status = LeaveStatusEnum.Approved;
            leaveRequest.ApprovedById = approverId;
            leaveRequest.ApprovedAt = DateTime.UtcNow;

            if (leaveRequest.LeaveType.IsPaid)
            {
                await UpdateAttendanceForLeave(leaveRequest);
            }

            await _db.SaveChangesAsync();

            var employeeUser = await _db.Users.FirstOrDefaultAsync(u => u.EmployeeId == leaveRequest.EmployeeId);

            if (employeeUser != null)
            {
                var emailTo = new EmailTo
                {
                    Mail = employeeUser.Email, Subject = "[HỆ THỐNG NHÂN SỰ] ĐƠN NGHỈ PHÉP ĐÃ ĐƯỢC DUYỆT", Content = $@" <h3>Xin chào {leaveRequest.Employee.FullName},</h3> <p style='color:green'><b>Đơn xin nghỉ phép của bạn đã được DUYỆT!</b></p> <table border='1' cellpadding='8'> <tr><td><b>Loại nghỉ</b></td><td>{leaveRequest.LeaveType.Name}</td></tr> <tr><td><b>Từ ngày</b></td><td>{leaveRequest.FromDate:dd/MM/yyyy}</td></tr> <tr><td><b>Đến ngày</b></td><td>{leaveRequest.ToDate:dd/MM/yyyy}</td></tr> <tr><td><b>Số ngày</b></td><td>{leaveRequest.TotalDays}</td></tr> <tr><td><b>Có lương</b></td><td>{(leaveRequest.LeaveType.IsPaid ? "Có" : "Không")}</td></tr> </table>" };
                await emailTo.SendEmailAsync(emailTo);
            }

            return _responseBase.ResponseBaseSuccess("Đã duyệt đơn nghỉ phép thành công!");
        }

        public async Task<ResponseBase> RejectAsync(int leaveRequestId, int approverId, Request_RejectLeaveRequest request)
        {
            var leaveRequest = await _db.LeaveRequests.Include(lr => lr.Employee).Include(lr => lr.LeaveType).FirstOrDefaultAsync(lr => lr.Id == leaveRequestId);

            if (leaveRequest == null) return _responseBase.ResponseBaseError(404, "Đơn nghỉ phép không tồn tại!");

            if (leaveRequest.Status != LeaveStatusEnum.Pending) return _responseBase.ResponseBaseError(400, $"Đơn này đã được xử lý (Trạng thái: {leaveRequest.Status})!");

            if (leaveRequest.Employee.ManagerId != approverId) return _responseBase.ResponseBaseError(403, "Bạn không có quyền từ chối đơn này (chỉ cấp trên trực tiếp mới được xử lý)!");

            leaveRequest.Status = LeaveStatusEnum.Rejected;
            leaveRequest.ApprovedById = approverId;
            leaveRequest.ApprovedAt = DateTime.UtcNow;
            leaveRequest.RejectReason = request.RejectReason;

            await _db.SaveChangesAsync();

            var employeeUser = await _db.Users.FirstOrDefaultAsync(u => u.EmployeeId == leaveRequest.EmployeeId);

            if (employeeUser != null)
            {
                var emailTo = new EmailTo
                {
                    Mail = employeeUser.Email, Subject = "[HỆ THỐNG NHÂN SỰ] ĐƠN NGHỈ PHÉP BỊ TỪ CHỐI", Content = $@" <h3>Xin chào {leaveRequest.Employee.FullName},</h3> <p style='color:red'><b>Đơn xin nghỉ phép của bạn đã bị TỪ CHỐI.</b></p> <table border='1' cellpadding='8'> <tr><td><b>Loại nghỉ</b></td><td>{leaveRequest.LeaveType.Name}</td></tr> <tr><td><b>Từ ngày</b></td><td>{leaveRequest.FromDate:dd/MM/yyyy}</td></tr> <tr><td><b>Đến ngày</b></td><td>{leaveRequest.ToDate:dd/MM/yyyy}</td></tr> <tr><td><b>Lý do từ chối</b></td><td>{request.RejectReason}</td></tr> </table>" };
                await emailTo.SendEmailAsync(emailTo);
            }

            return _responseBase.ResponseBaseSuccess("Đã từ chối đơn nghỉ phép!");
        }

        private async Task UpdateAttendanceForLeave(LeaveRequest lr)
        {
            var current = lr.FromDate.Date;
            var end = lr.ToDate.Date;

            while (current <= end)
            {
                if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
                {
                    var dateUtc = new DateTime(current.Year, current.Month, current.Day, 0, 0, 0, DateTimeKind.Utc);

                    var existing = await _db.Attendances.FirstOrDefaultAsync(a => a.EmployeeId == lr.EmployeeId && a.Date == dateUtc);

                    if (existing == null)
                    {
                        await _db.Attendances.AddAsync(new Attendance
                        {
                            EmployeeId = lr.EmployeeId, Date = dateUtc, Status = AttendanceStatusEnum.Leave, WorkingDays = 1.0m, WorkingHours = 0, Note = $"Nghỉ phép có lương (Đơn #{lr.Id})", CreatedAt = DateTime.UtcNow });
                    }
                    else if (existing.Status == AttendanceStatusEnum.Absent)
                    {
                        existing.Status = AttendanceStatusEnum.Leave;
                        existing.WorkingDays = 1.0m;
                        existing.Note = $"Nghỉ phép có lương (Đơn #{lr.Id})";
                    }
                }
                current = current.AddDays(1);
            }
        }

        public async Task<ResponseObject<PagedResult<DTO_LeaveRequest>>> GetAllAsync(int managerId, string role, int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            IQueryable<LeaveRequest> query = _db.LeaveRequests.Include(lr => lr.Employee).Include(lr => lr.LeaveType).Include(lr => lr.ApprovedBy);

            if (role != "Director")
            {
                var subordinateIds = await GetAllSubordinateIdsAsync(managerId);
                query = query.Where(lr => subordinateIds.Contains(lr.EmployeeId));
            }

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(lr => lr.Employee.EmployeeCode.Contains(search) || lr.Employee.FullName.Contains(search));

            int totalItems = await query.CountAsync();
            var items = await query.OrderByDescending(lr => lr.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = new PagedResult<DTO_LeaveRequest>
            {
                Items = items.Select(lr => _converter.ToDTO(lr)).ToList(), TotalItems = totalItems, TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize), CurrentPage = pageNumber };

            return _responsePagedList.ResponseObjectSuccess($"Có {totalItems} đơn nghỉ phép", result);
        }

        private async Task<List<int>> GetAllSubordinateIdsAsync(int managerId)
        {
            var result = new List<int>();
            var directSubs = await _db.Employees.Where(e => e.ManagerId == managerId && !e.IsDeleted).Select(e => e.Id).ToListAsync();

            foreach (var subId in directSubs)
            {
                result.Add(subId);
                var deeper = await GetAllSubordinateIdsAsync(subId);
                result.AddRange(deeper);
            }

            return result;
        }
    }
}