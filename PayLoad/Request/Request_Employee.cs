namespace HRemployee.PayLoad.Request
{
    public class Request_CreateEmployee
    {
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public int? ManagerId { get; set; }
    }

    public class Request_UpdateEmployee
    {
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public int? ManagerId { get; set; }
    }
}