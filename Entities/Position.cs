namespace HRemployee.Entities
{
    public class Position : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Employee> Employees { get; set; }
    }
}