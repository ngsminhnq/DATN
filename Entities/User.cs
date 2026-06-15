namespace HRemployee.Entities
{
    public class User : EntityBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}