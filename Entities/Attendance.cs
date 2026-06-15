using HRemployee.Enums;

namespace HRemployee.Entities
{
    public class Attendance : EntityBase
    {
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public decimal? WorkingHours { get; set; }
        public decimal? WorkingDays { get; set; }
        public AttendanceStatusEnum Status { get; set; } = AttendanceStatusEnum.Absent;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}