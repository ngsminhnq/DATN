using HRemployee.DataContext;
using HRemployee.Entities;
using HRemployee.PayLoad.DTO;
using Microsoft.EntityFrameworkCore;

namespace HRemployee.PayLoad.Converter
{
    public class Converter_Employee
    {
        private readonly AppDbContext _context;
        public Converter_Employee(AppDbContext context) { _context = context; }

        public DTO_Employee ToDTO(Employee e)
        {
            if (e == null) return null;

            var activeContract = e.Contracts?.Where(c => c.Status == Enums.ContractStatusEnum.Active).OrderByDescending(c => c.StartDate).FirstOrDefault();

            return new DTO_Employee
            {
                Id = e.Id, EmployeeCode = e.EmployeeCode, FullName = e.FullName, Email = e.Email, Phone = e.Phone, DateOfBirth = e.DateOfBirth, Gender = e.Gender, Address = e.Address, Avatar = e.Avatar, HireDate = e.HireDate, Status = e.Status.ToString(), DepartmentId = e.DepartmentId, DepartmentName = e.Department?.Name ?? "", PositionId = e.PositionId, PositionName = e.Position?.Name ?? "", ManagerId = e.ManagerId, ManagerName = e.Manager?.FullName, CurrentContractType = activeContract?.ContractType.ToString(), ContractEndDate = activeContract?.EndDate, CreatedAt = e.CreatedAt };
        }
    }
}