using HRemployee.DataContext;
using HRemployee.Entities;
using HRemployee.PayLoad.Converter;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;
using HRemployee.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace HRemployee.Service.Implement
{
    public class Service_SalaryConfig : IService_SalaryConfig
    {
        private readonly AppDbContext _db;
        private readonly Converter_SalaryConfig _converter;
        private readonly ResponseBase _responseBase;
        private readonly ResponseObject<DTO_SalaryConfig> _responseObj;
        private readonly ResponseObject<List<DTO_SalaryConfig>> _responseList;
        private readonly ResponseObject<PagedResult<DTO_SalaryConfig>> _responsePagedList;

        public Service_SalaryConfig(AppDbContext db, Converter_SalaryConfig converter, ResponseBase responseBase, ResponseObject<DTO_SalaryConfig> responseObj, ResponseObject<List<DTO_SalaryConfig>> responseList, ResponseObject<PagedResult<DTO_SalaryConfig>> responsePagedList)
        {
            _db           = db;
            _converter    = converter;
            _responseBase = responseBase;
            _responseObj  = responseObj;
            _responseList = responseList;
            _responsePagedList = responsePagedList;
        }

        public async Task<ResponseObject<PagedResult<DTO_SalaryConfig>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var query = _db.SalaryConfigs.Include(s => s.Employee).Where(s => s.IsActive && !s.Employee.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(s => s.Employee.EmployeeCode.Contains(search) || s.Employee.FullName.Contains(search));

            int totalItems = await query.CountAsync();
            var items = await query.OrderBy(s => s.Employee.EmployeeCode).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = new PagedResult<DTO_SalaryConfig>
            {
                Items = items.Select(s => _converter.ToDTO(s)).ToList(), TotalItems = totalItems, TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize), CurrentPage = pageNumber };

            return _responsePagedList.ResponseObjectSuccess("Lấy danh sách cấu hình lương thành công", result);
        }

        private async Task<Employee?> GetEmployeeAsync(string employeeCode)
            => await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode.ToUpper() && !e.IsDeleted);

        public async Task<ResponseObject<DTO_SalaryConfig>> CreateAsync(string employeeCode, Request_SalaryConfig request)
        {
            var employee = await GetEmployeeAsync(employeeCode);
            if (employee == null) return _responseObj.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);

            if (await _db.SalaryConfigs.AnyAsync(s => s.EmployeeId == employee.Id && s.IsActive)) return _responseObj.ResponseObjectError(400, $"Nhân viên '{employeeCode}' đã có cấu hình lương. Dùng Update để điều chỉnh.", null);

            if (request.BaseSalary <= 0) return _responseObj.ResponseObjectError(400, "Lương cơ bản phải lớn hơn 0", null);

            var config = new SalaryConfig
            {
                EmployeeId = employee.Id, BaseSalary = request.BaseSalary, Allowance = request.Allowance, KpiBonus = request.KpiBonus, EffectiveDate = DateTime.SpecifyKind(request.EffectiveDate.Date, DateTimeKind.Utc), IsActive = true, Note = request.Note, CreatedAt = DateTime.UtcNow };

            await _db.SalaryConfigs.AddAsync(config);
            await _db.SaveChangesAsync();

            var saved = await _db.SalaryConfigs.Include(s => s.Employee).FirstOrDefaultAsync(s => s.Id == config.Id);
            return _responseObj.ResponseObjectSuccess($"Đã tạo cấu hình lương cho {employeeCode} thành công!", _converter.ToDTO(saved!));
        }

        public async Task<ResponseObject<DTO_SalaryConfig>> UpdateAsync(string employeeCode, Request_SalaryConfig request)
        {
            var employee = await GetEmployeeAsync(employeeCode);
            if (employee == null) return _responseObj.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);

            if (request.BaseSalary <= 0) return _responseObj.ResponseObjectError(400, "Lương cơ bản phải lớn hơn 0", null);

            var activeConfigs = await _db.SalaryConfigs.Where(s => s.EmployeeId == employee.Id && s.IsActive).ToListAsync();
            foreach (var old in activeConfigs) old.IsActive = false;

            var newConfig = new SalaryConfig
            {
                EmployeeId = employee.Id, BaseSalary = request.BaseSalary, Allowance = request.Allowance, KpiBonus = request.KpiBonus, EffectiveDate = DateTime.SpecifyKind(request.EffectiveDate.Date, DateTimeKind.Utc), IsActive = true, Note = request.Note, CreatedAt = DateTime.UtcNow };

            await _db.SalaryConfigs.AddAsync(newConfig);
            await _db.SaveChangesAsync();

            var saved = await _db.SalaryConfigs.Include(s => s.Employee).FirstOrDefaultAsync(s => s.Id == newConfig.Id);
            return _responseObj.ResponseObjectSuccess($"Đã cập nhật cấu hình lương cho {employeeCode}. Config cũ đã vô hiệu.", _converter.ToDTO(saved!));
        }

        public async Task<ResponseObject<DTO_SalaryConfig>> GetCurrentAsync(string employeeCode)
        {
            var employee = await GetEmployeeAsync(employeeCode);
            if (employee == null) return _responseObj.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);

            var config = await _db.SalaryConfigs.Include(s => s.Employee).Where(s => s.EmployeeId == employee.Id && s.IsActive).FirstOrDefaultAsync();
            if (config == null) return _responseObj.ResponseObjectError(404, $"Nhân viên '{employeeCode}' chưa có cấu hình lương. Dùng Create để thiết lập.", null);

            return _responseObj.ResponseObjectSuccess("Thành công", _converter.ToDTO(config));
        }

        public async Task<ResponseObject<List<DTO_SalaryConfig>>> GetHistoryAsync(string employeeCode)
        {
            var employee = await GetEmployeeAsync(employeeCode);
            if (employee == null) return _responseList.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);

            var configs = await _db.SalaryConfigs.Include(s => s.Employee).Where(s => s.EmployeeId == employee.Id).OrderByDescending(s => s.EffectiveDate).ToListAsync();
            return _responseList.ResponseObjectSuccess($"Lịch sử cấu hình lương của {employeeCode}: {configs.Count} bản ghi", configs.Select(s => _converter.ToDTO(s)).ToList());
        }
    }
}