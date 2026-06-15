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
    public class Service_Contract : IService_Contract
    {
        private readonly AppDbContext _db;
        private readonly Converter_Contract _converter;
        private readonly ResponseBase _responseBase;
        private readonly ResponseObject<DTO_Contract> _responseObj;
        private readonly ResponseObject<List<DTO_Contract>> _responseList;
        private readonly ResponseObject<PagedResult<DTO_Contract>> _responsePagedList;

        public Service_Contract(
            AppDbContext db,
            Converter_Contract converter,
            ResponseBase responseBase,
            ResponseObject<DTO_Contract> responseObj,
            ResponseObject<List<DTO_Contract>> responseList,
            ResponseObject<PagedResult<DTO_Contract>> responsePagedList)
        {
            _db = db;
            _converter = converter;
            _responseBase = responseBase;
            _responseObj = responseObj;
            _responseList = responseList;
            _responsePagedList = responsePagedList;
        }

        public async Task<ResponseObject<PagedResult<DTO_Contract>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null)
        {
            var query = _db.Contracts.Include(c => c.Employee).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Employee.EmployeeCode.Contains(search) || c.Employee.FullName.Contains(search) || c.ContractCode.Contains(search));

            int totalItems = await query.CountAsync();
            var items = await query.OrderByDescending(c => c.StartDate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = new PagedResult<DTO_Contract>
            {
                Items = items.Select(c => _converter.ToDTO(c)).ToList(), TotalItems = totalItems, TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize), CurrentPage = pageNumber };

            return _responsePagedList.ResponseObjectSuccess("Lấy danh sách hợp đồng thành công", result);
        }

        public async Task<ResponseObject<List<DTO_Contract>>> GetByEmployeeAsync(int employeeId)
        {
            if (!await _db.Employees.AnyAsync(e => e.Id == employeeId && !e.IsDeleted)) return _responseList.ResponseObjectError(404, "Nhân viên không tồn tại", null);

            var contracts = await _db.Contracts.Include(c => c.Employee).Where(c => c.EmployeeId == employeeId).OrderByDescending(c => c.StartDate).ToListAsync();

            return _responseList.ResponseObjectSuccess("Thành công", contracts.Select(c => _converter.ToDTO(c)).ToList());
        }

        public async Task<ResponseObject<List<DTO_Contract>>> GetByEmployeeByCodeAsync(string employeeCode)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode.ToUpper() && !e.IsDeleted);
            if (employee == null) return _responseList.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);
            return await GetByEmployeeAsync(employee.Id);
        }

        public async Task<ResponseObject<DTO_Contract>> GetCurrentByCodeAsync(string employeeCode)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode.ToUpper() && !e.IsDeleted);
            if (employee == null) return _responseObj.ResponseObjectError(404, $"Không tìm thấy nhân viên '{employeeCode}'", null);
            return await GetCurrentAsync(employee.Id);
        }

        public async Task<ResponseObject<DTO_Contract>> GetCurrentAsync(int employeeId)
        {
            var contract = await _db.Contracts.Include(c => c.Employee).Where(c => c.EmployeeId == employeeId && c.Status == ContractStatusEnum.Active).OrderByDescending(c => c.StartDate).FirstOrDefaultAsync();

            if (contract == null) return _responseObj.ResponseObjectError(404, "Không có hợp đồng đang hiệu lực", null);

            return _responseObj.ResponseObjectSuccess("Thành công", _converter.ToDTO(contract));
        }

        public async Task<ResponseObject<DTO_Contract>> CreateAsync(Request_CreateContract request)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == request.EmployeeCode && !e.IsDeleted);

            if (employee == null) return _responseObj.ResponseObjectError(404, $"Không tìm thấy nhân viên với mã '{request.EmployeeCode}'", null);

            if (request.EndDate <= request.StartDate) return _responseObj.ResponseObjectError(400, "Ngày kết thúc phải sau ngày bắt đầu", null);

            if (await _db.Contracts.AnyAsync(c => c.EmployeeId == employee.Id && c.Status == ContractStatusEnum.Active)) return _responseObj.ResponseObjectError(400, "Nhân viên đã có hợp đồng đang hiệu lực. Hãy gia hạn hoặc chấm dứt trước.", null);

            var lastContract = await _db.Contracts.OrderByDescending(c => c.Id).FirstOrDefaultAsync();
            string code = $"HD{(lastContract?.Id + 1 ?? 1):D4}";

            var contract = new Contract
            {
                ContractCode = code, EmployeeId = employee.Id, ContractType = request.ContractType, SalaryPercent = request.SalaryPercent, StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc), EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc), Status = ContractStatusEnum.Active, Note = request.Note, CreatedAt = DateTime.UtcNow };

            await _db.Contracts.AddAsync(contract);
            await _db.SaveChangesAsync();

            var saved = await _db.Contracts.Include(c => c.Employee).FirstOrDefaultAsync(c => c.Id == contract.Id);
            return _responseObj.ResponseObjectSuccess("Ký hợp đồng thành công", _converter.ToDTO(saved));
        }

        public async Task<ResponseObject<DTO_Contract>> RenewAsync(int contractId, Request_RenewContract request)
        {
            var oldContract = await _db.Contracts.Include(c => c.Employee).FirstOrDefaultAsync(c => c.Id == contractId);

            if (oldContract == null) return _responseObj.ResponseObjectError(404, "Hợp đồng không tồn tại", null);

            if (oldContract.Status != ContractStatusEnum.Active) return _responseObj.ResponseObjectError(400, "Hợp đồng không còn hiệu lực để gia hạn", null);

            oldContract.Status = ContractStatusEnum.Expired;

            var lastContract = await _db.Contracts.OrderByDescending(c => c.Id).FirstOrDefaultAsync();
            string code = $"HD{(lastContract?.Id + 1 ?? 1):D4}";

            var newContract = new Contract
            {
                ContractCode = code, EmployeeId = oldContract.EmployeeId, ContractType = request.NewContractType ?? oldContract.ContractType, SalaryPercent = request.NewSalaryPercent ?? oldContract.SalaryPercent, StartDate = oldContract.EndDate, EndDate = DateTime.SpecifyKind(request.NewEndDate, DateTimeKind.Utc), Status = ContractStatusEnum.Active, Note = request.Note ?? $"Gia hạn từ hợp đồng {oldContract.ContractCode}", CreatedAt = DateTime.UtcNow };

            await _db.Contracts.AddAsync(newContract);
            await _db.SaveChangesAsync();

            var saved = await _db.Contracts.Include(c => c.Employee).FirstOrDefaultAsync(c => c.Id == newContract.Id);

            return _responseObj.ResponseObjectSuccess( $"Gia hạn thành công! Hợp đồng mới: {code}", _converter.ToDTO(saved));
        }

        public async Task<ResponseBase> TerminateAsync(int contractId, Request_TerminateContract request)
        {
            var contract = await _db.Contracts.FirstOrDefaultAsync(c => c.Id == contractId);

            if (contract == null) return _responseBase.ResponseBaseError(404, "Hợp đồng không tồn tại");

            if (contract.Status != ContractStatusEnum.Active) return _responseBase.ResponseBaseError(400, "Hợp đồng không còn hiệu lực");

            contract.Status = ContractStatusEnum.Terminated;
            contract.TerminatedAt = DateTime.UtcNow;
            contract.Note = (contract.Note ?? "") + $"\nLý do chấm dứt: {request.Reason}";

            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == contract.EmployeeId);
            if (employee != null)
            {
                employee.Status = EmployeeStatusEnum.Inactive;
                employee.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            return _responseBase.ResponseBaseSuccess("Chấm dứt hợp đồng thành công");
        }

        public async Task<ResponseObject<DTO_Contract>> RenewByCodeAsync(string contractCode, Request_RenewContract request)
        {
            var contract = await _db.Contracts.FirstOrDefaultAsync(c => c.ContractCode == contractCode.ToUpper());
            if (contract == null) return _responseObj.ResponseObjectError(404, $"Không tìm thấy hợp đồng '{contractCode}'", null);
            return await RenewAsync(contract.Id, request);
        }

        public async Task<ResponseBase> TerminateByCodeAsync(string contractCode, Request_TerminateContract request)
        {
            var contract = await _db.Contracts.FirstOrDefaultAsync(c => c.ContractCode == contractCode.ToUpper());
            if (contract == null) return _responseBase.ResponseBaseError(404, $"Không tìm thấy hợp đồng '{contractCode}'");
            return await TerminateAsync(contract.Id, request);
        }
    }
}