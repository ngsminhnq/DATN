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
    public class Service_Position : IService_Position
    {
        private readonly AppDbContext _context;
        private readonly Converter_Position _converter;
        private readonly ResponseObject<DTO_Position> _responseObject;
        private readonly ResponseObject<PagedResult<DTO_Position>> _responseList;
        private readonly ResponseBase _responseBase;

        public Service_Position(
            AppDbContext context,
            Converter_Position converter,
            ResponseObject<DTO_Position> responseObject,
            ResponseObject<PagedResult<DTO_Position>> responseList,
            ResponseBase responseBase)
        {
            _context = context;
            _converter = converter;
            _responseObject = responseObject;
            _responseList = responseList;
            _responseBase = responseBase;
        }

        public async Task<ResponseObject<PagedResult<DTO_Position>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var query = _context.Positions.Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || p.Code.Contains(search));

            int totalItems = await query.CountAsync();
            var positions = await query.OrderBy(p => p.Name).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var items = new List<DTO_Position>();
            foreach (var pos in positions)
            {
                var count = await _context.Employees.CountAsync(e => e.PositionId == pos.Id && !e.IsDeleted);
                items.Add(_converter.ToDTO(pos, count));
            }

            var result = new PagedResult<DTO_Position>
            {
                Items = items, TotalItems = totalItems, TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize), CurrentPage = pageNumber };

            return _responseList.ResponseObjectSuccess("Lấy danh sách chức vụ thành công", result);
        }

        public async Task<ResponseObject<DTO_Position>> GetByIdAsync(int id)
        {
            var pos = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (pos == null) return _responseObject.ResponseObjectError(404, "Không tìm thấy chức vụ", null);

            var count = await _context.Employees.CountAsync(e => e.PositionId == id && !e.IsDeleted);
            return _responseObject.ResponseObjectSuccess("Thành công", _converter.ToDTO(pos, count));
        }

        public async Task<ResponseObject<DTO_Position>> CreateAsync(Request_CreatePosition request)
        {
            if (await _context.Positions.AnyAsync(p => p.Code == request.Code && !p.IsDeleted)) return _responseObject.ResponseObjectError(400, $"Mã chức vụ '{request.Code}' đã tồn tại", null);

            var pos = new Position
            {
                Name = request.Name, Code = request.Code.ToUpper(), Description = request.Description, CreatedAt = DateTime.UtcNow };

            await _context.Positions.AddAsync(pos);
            await _context.SaveChangesAsync();

            return _responseObject.ResponseObjectSuccess("Tạo chức vụ thành công", _converter.ToDTO(pos));
        }

        public async Task<ResponseObject<DTO_Position>> UpdateAsync(int id, Request_UpdatePosition request)
        {
            var pos = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (pos == null) return _responseObject.ResponseObjectError(404, "Không tìm thấy chức vụ", null);

            pos.Name = request.Name;
            pos.Description = request.Description;

            await _context.SaveChangesAsync();

            var count = await _context.Employees.CountAsync(e => e.PositionId == id && !e.IsDeleted);
            return _responseObject.ResponseObjectSuccess("Cập nhật chức vụ thành công", _converter.ToDTO(pos, count));
        }

        public async Task<ResponseBase> DeleteAsync(int id)
        {
            var pos = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (pos == null) return _responseBase.ResponseBaseError(404, "Không tìm thấy chức vụ");

            var hasEmployee = await _context.Employees.AnyAsync(e => e.PositionId == id && !e.IsDeleted);
            if (hasEmployee)
                return _responseBase.ResponseBaseError(400, "Không thể xóa chức vụ đang có nhân viên");

            pos.IsDeleted = true;
            await _context.SaveChangesAsync();

            return _responseBase.ResponseBaseSuccess("Xóa chức vụ thành công");
        }

        public async Task<ResponseObject<DTO_Position>> GetByCodeAsync(string code)
        {
            var pos = await _context.Positions.FirstOrDefaultAsync(p => p.Code == code.ToUpper() && !p.IsDeleted);
            if (pos == null) return _responseObject.ResponseObjectError(404, $"Không tìm thấy chức vụ '{code}'", null);
            return await GetByIdAsync(pos.Id);
        }

        public async Task<ResponseObject<DTO_Position>> UpdateByCodeAsync(string code, Request_UpdatePosition request)
        {
            var pos = await _context.Positions.FirstOrDefaultAsync(p => p.Code == code.ToUpper() && !p.IsDeleted);
            if (pos == null) return _responseObject.ResponseObjectError(404, $"Không tìm thấy chức vụ '{code}'", null);
            return await UpdateAsync(pos.Id, request);
        }

        public async Task<ResponseBase> DeleteByCodeAsync(string code)
        {
            var pos = await _context.Positions.FirstOrDefaultAsync(p => p.Code == code.ToUpper() && !p.IsDeleted);
            if (pos == null) return _responseBase.ResponseBaseError(404, $"Không tìm thấy chức vụ '{code}'");
            return await DeleteAsync(pos.Id);
        }
    }
}