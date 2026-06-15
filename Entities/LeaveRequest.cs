using HRemployee.Enums;

namespace HRemployee.Entities
{
    public class LeaveRequest : EntityBase
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalDays { get; set; }
        public string Reason { get; set; }
        public LeaveStatusEnum Status { get; set; } = LeaveStatusEnum.Pending;
        public DateTime? ApprovedAt { get; set; }
        public string? RejectReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int LeaveTypeId { get; set; }
        public LeaveType LeaveType { get; set; }

        public int? ApprovedById { get; set; }
        public Employee ApprovedBy { get; set; }
    }
}