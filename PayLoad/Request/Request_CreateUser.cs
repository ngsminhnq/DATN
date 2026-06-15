namespace HRemployee.PayLoad.Request
{
    public class Request_CreateUser
    {
        public string Username { get; set; }
        public int RoleId { get; set; }
        public string EmployeeCode { get; set; }
    }
}