using HRemployee.Entities;
using HRemployee.PayLoad.DTO;

namespace HRemployee.PayLoad.Converter
{
    public class Converter_Attendance
    {
        public DTO_Attendance ToDTO(Attendance a)
        {
            if (a == null) return null;
            return new DTO_Attendance
            {
                Id = a.Id, Date = a.Date, CheckInTime = a.CheckInTime, CheckOutTime = a.CheckOutTime, WorkingHours = a.WorkingHours, WorkingDays = a.WorkingDays, Status = a.Status.ToString(), Note = a.Note, EmployeeId = a.EmployeeId, EmployeeName = a.Employee?.FullName ?? "" };
        }
    }
}