namespace HRemployee.PayLoad.Request
{
    public class Request_CreatePosition
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
    }

    public class Request_UpdatePosition
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}