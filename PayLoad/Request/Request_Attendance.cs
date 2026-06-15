using System.ComponentModel;

namespace HRemployee.PayLoad.Request
{
    public class Request_CheckIn
    {
        public string? Note { get; set; }
    }

    public class Request_CheckOut
    {
        public string? Note { get; set; }
    }

    public class Request_AttendanceSummary
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int? EmployeeId { get; set; }
    }

    public class Request_EditAttendance
    {
        [DefaultValue("12/05/2026 08:00:00")]
        public DateTime? CheckInTime { get; set; }
        [DefaultValue("12/05/2026 17:00:00")]
        public DateTime? CheckOutTime { get; set; }
        [DefaultValue("Present")]
        public string? Status { get; set; }
        [DefaultValue("Sửa thủ công - đủ 8 tiếng")]
        public string? Note { get; set; }
    }
}