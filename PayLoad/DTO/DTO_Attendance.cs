namespace HRemployee.PayLoad.DTO
{
    public class DTO_Attendance
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public decimal? WorkingHours { get; set; }
        public decimal? WorkingDays { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
    }

    public class DTO_AttendanceSummary
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalWorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int LateDays { get; set; }
        public int AbsentDays { get; set; }
        public int LeaveDays { get; set; }
        public decimal TotalWorkingHours { get; set; }
    }
}