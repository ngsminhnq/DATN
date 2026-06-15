using HRemployee.Enums;

namespace HRemployee.Entities
{
    public class Employee : EntityBase
    {
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime HireDate { get; set; }
        public EmployeeStatusEnum Status { get; set; } = EmployeeStatusEnum.Active;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }

        public int? ManagerId { get; set; }
        public Employee Manager { get; set; }

        public User User { get; set; }
        public ICollection<Employee> Subordinates { get; set; }
        public ICollection<Contract> Contracts { get; set; }
        public ICollection<SalaryConfig> SalaryConfigs { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
        public ICollection<LeaveRequest> LeaveRequests { get; set; }
        public ICollection<LeaveRequest> ApprovedLeaveRequests { get; set; }
        public ICollection<SalaryRecord> SalaryRecords { get; set; }
    }
}