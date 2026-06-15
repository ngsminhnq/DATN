using HRemployee.DataContext;
using HRemployee.Entities;
using HRemployee.PayLoad.DTO;
using HRemployee.PayLoad.Request;
using HRemployee.PayLoad.Response;
using HRemployee.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace HRemployee.Service.Implement
{
    public class Service_LeaveType : IService_LeaveType
    {
        private readonly AppDbContext _db;
        private readonly ResponseObject<DTO_LeaveType> _responseObject;
        private readonly ResponseObject<List<DTO_LeaveType>> _responseList;
        private readonly ResponseBase _responseBase;

        public Service_LeaveType(
            AppDbContext db,
            ResponseObject<DTO_LeaveType> responseObject,
            ResponseObject<List<DTO_LeaveType>> responseList,
            ResponseBase responseBase)
        {
            _db = db;
            _responseObject = responseObject;
            _responseList = responseList;
            _responseBase = responseBase;
        }

        private static DTO_LeaveType ToDTO(LeaveType lt) => new DTO_LeaveType
        {
            Id = lt.Id, Name = lt.Name, IsPaid = lt.IsPaid, Description = lt.Description };

        public async Task<ResponseObject<List<DTO_LeaveType>>> GetAllAsync(string? search = null)
        {
            var query = _db.LeaveTypes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(lt => lt.Name.Contains(search));

            var list = await query.OrderBy(lt => lt.Name).ToListAsync();

            return _responseList.ResponseObjectSuccess( "Lấy danh sách loại nghỉ phép thành công", list.Select(ToDTO).ToList());
        }

        public async Task<ResponseObject<DTO_LeaveType>> CreateAsync(Request_CreateLeaveType request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return _responseObject.ResponseObjectError(400, "Tên loại nghỉ phép không được để trống", null);

            var exists = await _db.LeaveTypes.AnyAsync(lt => lt.Name == request.Name.Trim());
            if (exists)
                return _responseObject.ResponseObjectError(400, $"Loại nghỉ phép '{request.Name}' đã tồn tại", null);

            var leaveType = new LeaveType
            {
                Name = request.Name.Trim(), IsPaid = request.IsPaid, Description = request.Description };

            _db.LeaveTypes.Add(leaveType);
            await _db.SaveChangesAsync();

            return _responseObject.ResponseObjectSuccess("Thêm loại nghỉ phép thành công", ToDTO(leaveType));
        }

        public async Task<ResponseObject<DTO_LeaveType>> UpdateAsync(int id, Request_UpdateLeaveType request)
        {
            var leaveType = await _db.LeaveTypes.FirstOrDefaultAsync(lt => lt.Id == id);
            if (leaveType == null) return _responseObject.ResponseObjectError(404, "Không tìm thấy loại nghỉ phép", null);

            if (string.IsNullOrWhiteSpace(request.Name))
                return _responseObject.ResponseObjectError(400, "Tên loại nghỉ phép không được để trống", null);

            var duplicate = await _db.LeaveTypes.AnyAsync(lt => lt.Name == request.Name.Trim() && lt.Id != id);
            if (duplicate)
                return _responseObject.ResponseObjectError(400, $"Tên '{request.Name}' đã được sử dụng", null);

            leaveType.Name = request.Name.Trim();
            leaveType.IsPaid = request.IsPaid;
            leaveType.Description = request.Description;

            await _db.SaveChangesAsync();

            return _responseObject.ResponseObjectSuccess("Cập nhật loại nghỉ phép thành công", ToDTO(leaveType));
        }

        public async Task<ResponseBase> DeleteAsync(int id)
        {
            var leaveType = await _db.LeaveTypes.FirstOrDefaultAsync(lt => lt.Id == id);
            if (leaveType == null) return _responseBase.ResponseBaseError(404, "Không tìm thấy loại nghỉ phép");

            var inUse = await _db.LeaveRequests.AnyAsync(lr => lr.LeaveTypeId == id);
            if (inUse)
                return _responseBase.ResponseBaseError(400, "Không thể xóa loại nghỉ phép đang được sử dụng");

            _db.LeaveTypes.Remove(leaveType);
            await _db.SaveChangesAsync();

            return _responseBase.ResponseBaseSuccess("Xóa loại nghỉ phép thành công");
        }
    }
}