namespace HRemployee.PayLoad.Request
{
    public class Request_CreateDepartment
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
    }

    public class Request_UpdateDepartment
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}