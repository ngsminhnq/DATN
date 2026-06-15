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
    public class Service_Department : IService_Department
    {
        private readonly AppDbContext _context;
        private readonly Converter_Department _converter;
        private readonly ResponseObject<DTO_Department> _responseObject;
        private readonly ResponseObject<PagedResult<DTO_Department>> _responseList;
        private readonly ResponseBase _responseBase;

        public Service_Department(
            AppDbContext context,
            Converter_Department converter,
            ResponseObject<DTO_Department> responseObject,
            ResponseObject<PagedResult<DTO_Department>> responseList,
            ResponseBase responseBase)
        {
            _context = context;
            _converter = converter;
            _responseObject = responseObject;
            _responseList = responseList;
            _responseBase = responseBase;
        }

        public async Task<ResponseObject<PagedResult<DTO_Department>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var query = _context.Departments.Where(d => !d.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d => d.Name.Contains(search) || d.Code.Contains(search));

            int totalItems = await query.CountAsync();
            var departments = await query.OrderBy(d => d.Name).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var items = new List<DTO_Department>();
            foreach (var dept in departments)
            {
                var count = await _context.Employees.CountAsync(e => e.DepartmentId == dept.Id && !e.IsDeleted);
                items.Add(_converter.ToDTO(dept, count));
            }

            var result = new PagedResult<DTO_Department>
            {
                Items = items, TotalItems = totalItems, TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize), CurrentPage = pageNumber };

            return _responseList.ResponseObjectSuccess("Lấy danh sách phòng ban thành công", result);
        }

        public async Task<ResponseObject<DTO_Department>> GetByIdAsync(int id)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
            if (dept == null) return _responseObject.ResponseObjectError(404, "Không tìm thấy phòng ban", null);

            var count = await _context.Employees.CountAsync(e => e.DepartmentId == id && !e.IsDeleted);
            return _responseObject.ResponseObjectSuccess("Thành công", _converter.ToDTO(dept, count));
        }

        public async Task<ResponseObject<DTO_Department>> CreateAsync(Request_CreateDepartment request)
        {
            if (await _context.Departments.AnyAsync(d => d.Code == request.Code && !d.IsDeleted)) return _responseObject.ResponseObjectError(400, $"Mã phòng ban '{request.Code}' đã tồn tại", null);

            var dept = new Department
            {
                Name = request.Name, Code = request.Code.ToUpper(), Description = request.Description, CreatedAt = DateTime.UtcNow };

            await _context.Departments.AddAsync(dept);
            await _context.SaveChangesAsync();

            return _responseObject.ResponseObjectSuccess("Tạo phòng ban thành công", _converter.ToDTO(dept));
        }

        public async Task<ResponseObject<DTO_Department>> UpdateAsync(int id, Request_UpdateDepartment request)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
            if (dept == null) return _responseObject.ResponseObjectError(404, "Không tìm thấy phòng ban", null);

            dept.Name = request.Name;
            dept.Description = request.Description;

            await _context.SaveChangesAsync();

            var count = await _context.Employees.CountAsync(e => e.DepartmentId == id && !e.IsDeleted);
            return _responseObject.ResponseObjectSuccess("Cập nhật phòng ban thành công", _converter.ToDTO(dept, count));
        }

        public async Task<ResponseBase> DeleteAsync(int id)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
            if (dept == null) return _responseBase.ResponseBaseError(404, "Không tìm thấy phòng ban");

            var hasEmployee = await _context.Employees.AnyAsync(e => e.DepartmentId == id && !e.IsDeleted);
            if (hasEmployee)
                return _responseBase.ResponseBaseError(400, "Không thể xóa phòng ban đang có nhân viên");

            dept.IsDeleted = true;
            await _context.SaveChangesAsync();

            return _responseBase.ResponseBaseSuccess("Xóa phòng ban thành công");
        }

        public async Task<ResponseObject<DTO_Department>> GetByCodeAsync(string code)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(d => d.Code == code.ToUpper() && !d.IsDeleted);
            if (dept == null) return _responseObject.ResponseObjectError(404, $"Không tìm thấy phòng ban '{code}'", null);
            return await GetByIdAsync(dept.Id);
        }

        public async Task<ResponseObject<DTO_Department>> UpdateByCodeAsync(string code, Request_UpdateDepartment request)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(d => d.Code == code.ToUpper() && !d.IsDeleted);
            if (dept == null) return _responseObject.ResponseObjectError(404, $"Không tìm thấy phòng ban '{code}'", null);
            return await UpdateAsync(dept.Id, request);
        }

        public async Task<ResponseBase> DeleteByCodeAsync(string code)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(d => d.Code == code.ToUpper() && !d.IsDeleted);
            if (dept == null) return _responseBase.ResponseBaseError(404, $"Không tìm thấy phòng ban '{code}'");
            return await DeleteAsync(dept.Id);
        }
    }
}