using HRemployee.Entities;
using HRemployee.PayLoad.DTO;

namespace HRemployee.PayLoad.Converter
{
    public class Converter_LeaveRequest
    {
        public DTO_LeaveRequest ToDTO(LeaveRequest lr)
        {
            if (lr == null) return null;

            return new DTO_LeaveRequest
            {
                Id = lr.Id,

                EmployeeId = lr.EmployeeId,
                EmployeeName = lr.Employee?.FullName ?? "",
                EmployeeCode = lr.Employee?.EmployeeCode,

                LeaveTypeId = lr.LeaveTypeId,
                LeaveTypeName = lr.LeaveType?.Name ?? "",
                IsPaid = lr.LeaveType?.IsPaid ?? false,

                FromDate = lr.FromDate,
                ToDate = lr.ToDate,
                TotalDays = lr.TotalDays,

                Reason = lr.Reason,
                Status = lr.Status.ToString(),
                ApprovedById = lr.ApprovedById,
                ApprovedByName = lr.ApprovedBy?.FullName,
                ApprovedAt = lr.ApprovedAt,
                RejectReason = lr.RejectReason,

                CreatedAt = lr.CreatedAt };
        }
    }
}