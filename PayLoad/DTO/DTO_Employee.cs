using HRemployee.Enums;

namespace HRemployee.PayLoad.DTO
{
    public class DTO_Employee
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime HireDate { get; set; }
        public string Status { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int PositionId { get; set; }
        public string PositionName { get; set; }

        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }

        public string? CurrentContractType { get; set; }
        public DateTime? ContractEndDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}