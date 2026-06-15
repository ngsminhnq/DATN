namespace HRemployee.PayLoad.DTO
{
    public class DTO_LeaveType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsPaid { get; set; }
        public string? Description { get; set; }
    }
}