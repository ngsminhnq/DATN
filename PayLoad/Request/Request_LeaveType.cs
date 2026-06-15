namespace HRemployee.PayLoad.Request
{
    public class Request_CreateLeaveType
    {
        public string Name { get; set; }
        public bool IsPaid { get; set; }
        public string? Description { get; set; }
    }

    public class Request_UpdateLeaveType
    {
        public string Name { get; set; }
        public bool IsPaid { get; set; }
        public string? Description { get; set; }
    }
}