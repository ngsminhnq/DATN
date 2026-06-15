using System.ComponentModel.DataAnnotations;

namespace HRemployee.PayLoad.Request
{
    public class Request_CreateLeaveRequest
    {
        [Required(ErrorMessage = "Vui lòng chọn loại nghỉ phép!")]
        public int LeaveTypeId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu!")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc!")]
        public DateTime ToDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do xin nghỉ!")]
        [MaxLength(1000)]
        public string Reason { get; set; }
    }

    public class Request_RejectLeaveRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập lý do từ chối!")]
        [MaxLength(500)]
        public string RejectReason { get; set; }
    }
}