namespace HRemployee.PayLoad.DTO
{
    public class DTO_LeaveRequest
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }

        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public bool IsPaid { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public int? ApprovedById { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectReason { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}