namespace HRemployee.Entities
{

    public class SalaryConfig : EntityBase
    {
        public decimal BaseSalary { get; set; }
        public decimal Allowance { get; set; } = 0;
        public decimal KpiBonus { get; set; } = 0;
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}