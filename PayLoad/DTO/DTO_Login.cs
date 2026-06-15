namespace HRemployee.PayLoad.DTO
{
    public class DTO_Login
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int? EmployeeId { get; set; }
        public string? FullName { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}