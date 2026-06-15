using HRemployee.DataContext;
using HRemployee.Entities;
using HRemployee.Enums;
using HRemployee.Helper;
using HRemployee.PayLoad.Converter;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;
using HRemployee.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HRemployee.Service.Implement
{
    public class Service_Employee : IService_Employee
    {
        private readonly AppDbContext _db;
        private readonly Converter_Employee _converter;
        private readonly CloudinaryService _cloudinary;
        private readonly ResponseBase _responseBase;
        private readonly ResponseObject<DTO_Employee> _responseObj;
        private readonly ResponseObject<PagedResult<DTO_Employee>> _responseList;

        public Service_Employee(
            AppDbContext db,
            Converter_Employee converter,
            CloudinaryService cloudinary,
            ResponseBase responseBase,
            ResponseObject<DTO_Employee> responseObj,
            ResponseObject<PagedResult<DTO_Employee>> responseList)
        {
            _db = db;
            _converter = converter;
            _cloudinary = cloudinary;
            _responseBase = responseBase;
            _responseObj = responseObj;
            _responseList = responseList;
        }

        public async Task<ResponseObject<PagedResult<DTO_Employee>>> GetAllAsync(
            int pageNumber, int pageSize,
            string? search = null)
        {
            var query = _db.Employees.Include(e => e.Department).Include(e => e.Position).Include(e => e.Manager).Include(e => e.Contracts).Where(e => !e.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.FullName.Contains(search) || e.EmployeeCode.Contains(search) || e.Email.Contains(search));

            int totalItems = await query.CountAsync();
            var items = await query.OrderBy(e => e.EmployeeCode).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = new PagedResult<DTO_Employee>
            {
                Items = items.Select(e => _converter.ToDTO(e)).ToList(), TotalItems = totalItems, TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize), CurrentPage = pageNumber };

            return _responseList.ResponseObjectSuccess("Lấy danh sách nhân viên thành công", result);
        }

        public async Task<ResponseObject<DTO_Employee>> GetByIdAsync(int id)
        {
            var employee = await GetEmployeeWithIncludes(id);
            if (employee == null) return _responseObj.ResponseObjectError(404, "Không tìm thấy nhân viên", null);

            return _responseObj.ResponseObjectSuccess("Thành công", _converter.ToDTO(employee));
        }

        public async Task<ResponseObject<DTO_Employee>> GetByCodeAsync(string employeeCode)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode.ToUpper() && !e.IsDeleted);
            if (employee == null) return _responseObj.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);
            return await GetByIdAsync(employee.Id);
        }

        public async Task<ResponseObject<DTO_Employee>> CreateAsync(Request_CreateEmployee request)
        {
            if (await _db.Employees.AnyAsync(e => e.EmployeeCode == request.EmployeeCode && !e.IsDeleted)) return _responseObj.ResponseObjectError(400, $"Mã nhân viên '{request.EmployeeCode}' đã tồn tại", null);

            if (await _db.Employees.AnyAsync(e => e.Email == request.Email && !e.IsDeleted)) return _responseObj.ResponseObjectError(400, $"Email '{request.Email}' đã tồn tại", null);

            if (!await _db.Departments.AnyAsync(d => d.Id == request.DepartmentId && !d.IsDeleted)) return _responseObj.ResponseObjectError(404, "Phòng ban không tồn tại", null);
            if (!await _db.Positions.AnyAsync(p => p.Id == request.PositionId && !p.IsDeleted)) return _responseObj.ResponseObjectError(404, "Chức vụ không tồn tại", null);

            var employee = new Employee
            {
                EmployeeCode = request.EmployeeCode.ToUpper(), FullName = request.FullName, Email = request.Email, Phone = request.Phone, DateOfBirth = request.DateOfBirth.HasValue ? DateTime.SpecifyKind(request.DateOfBirth.Value, DateTimeKind.Utc) : null, Gender = request.Gender, Address = request.Address, HireDate = DateTime.SpecifyKind(request.HireDate, DateTimeKind.Utc), DepartmentId = request.DepartmentId, PositionId = request.PositionId, ManagerId = request.ManagerId, Status = EmployeeStatusEnum.Active, Avatar = "https://res.cloudinary.com/dkh1dujcl/image/upload/v1/default_avatar.png", CreatedAt = DateTime.UtcNow };

            await _db.Employees.AddAsync(employee);
            await _db.SaveChangesAsync();

            var saved = await GetEmployeeWithIncludes(employee.Id);
            return _responseObj.ResponseObjectSuccess("Tạo nhân viên thành công", _converter.ToDTO(saved));
        }

        public async Task<ResponseObject<DTO_Employee>> UpdateAsync(int id, Request_UpdateEmployee request)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            if (employee == null) return _responseObj.ResponseObjectError(404, "Không tìm thấy nhân viên", null);

            employee.Phone = request.Phone;
            employee.Address = request.Address;
            employee.DepartmentId = request.DepartmentId;
            employee.PositionId = request.PositionId;
            employee.ManagerId = request.ManagerId;
            employee.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            var saved = await GetEmployeeWithIncludes(employee.Id);
            return _responseObj.ResponseObjectSuccess("Cập nhật nhân viên thành công", _converter.ToDTO(saved));
        }

        public async Task<ResponseObject<DTO_Employee>> UploadAvatarAsync(int id, IFormFile avatar)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            if (employee == null) return _responseObj.ResponseObjectError(404, "Không tìm thấy nhân viên", null);

            if (!CheckInput.IsImage(avatar))
                return _responseObj.ResponseObjectError(400, "File không phải ảnh hợp lệ!", null);

            string newUrl;
            if (!string.IsNullOrEmpty(employee.Avatar) && employee.Avatar.Contains("cloudinary") && !employee.Avatar.Contains("default_avatar"))
            {
                newUrl = await _cloudinary.ReplaceImage(employee.Avatar, avatar);
            }
            else
            {
                newUrl = await _cloudinary.UploadImage(avatar);
            }

            employee.Avatar = newUrl;
            employee.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var saved = await GetEmployeeWithIncludes(employee.Id);
            return _responseObj.ResponseObjectSuccess("Cập nhật avatar thành công", _converter.ToDTO(saved));
        }

        public async Task<ResponseBase> DeleteAsync(int id)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            if (employee == null) return _responseBase.ResponseBaseError(404, "Không tìm thấy nhân viên");

            if (await _db.Contracts.AnyAsync(c => c.EmployeeId == id && c.Status == ContractStatusEnum.Active)) return _responseBase.ResponseBaseError(400, "Không thể xóa nhân viên đang có hợp đồng còn hiệu lực");

            if (await _db.Employees.AnyAsync(e => e.ManagerId == id && !e.IsDeleted)) return _responseBase.ResponseBaseError(400, "Không thể xóa nhân viên đang quản lý người khác");

            employee.IsDeleted = true;
            employee.Status = EmployeeStatusEnum.Inactive;
            employee.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return _responseBase.ResponseBaseSuccess("Xóa nhân viên thành công");
        }

        public async Task<ResponseObject<DTO_Employee>> UpdateByCodeAsync(string employeeCode, Request_UpdateEmployee request)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode.ToUpper() && !e.IsDeleted);
            if (employee == null) return _responseObj.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);
            return await UpdateAsync(employee.Id, request);
        }

        public async Task<ResponseObject<DTO_Employee>> UploadAvatarByCodeAsync(string employeeCode, IFormFile avatar)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode.ToUpper() && !e.IsDeleted);
            if (employee == null) return _responseObj.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);
            return await UploadAvatarAsync(employee.Id, avatar);
        }

        public async Task<ResponseBase> DeleteByCodeAsync(string employeeCode)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode.ToUpper() && !e.IsDeleted);
            if (employee == null) return _responseBase.ResponseBaseError(404, $"Không tìm thấy nhân viên '{employeeCode}'");
            return await DeleteAsync(employee.Id);
        }

        private async Task<Employee?> GetEmployeeWithIncludes(int id)
        {
            return await _db.Employees.Include(e => e.Department).Include(e => e.Position).Include(e => e.Manager).Include(e => e.Contracts).FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }
    }
}