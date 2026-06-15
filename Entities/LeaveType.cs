namespace HRemployee.Entities
{
    public class LeaveType : EntityBase
    {
        public string Name { get; set; }
        public bool IsPaid { get; set; }
        public string? Description { get; set; }

        public ICollection<LeaveRequest> LeaveRequests { get; set; }
    }
}