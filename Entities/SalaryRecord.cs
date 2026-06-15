using HRemployee.Enums;

namespace HRemployee.Entities
{

    public class SalaryRecord : EntityBase
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public decimal BaseSalary { get; set; }
        public decimal Allowance { get; set; } = 0;
        public decimal KpiBonus { get; set; } = 0;
        public decimal KpiCoefficient { get; set; } = 1.0m;
        public int SalaryPercent { get; set; } = 100;

        public int StandardDays { get; set; }
        public decimal WorkingDays { get; set; }

        public decimal GrossSalary { get; set; }
        public decimal InsuranceDeduction { get; set; } = 0;
        public decimal TaxDeduction { get; set; } = 0;
        public decimal NetSalary { get; set; }

        public SalaryStatusEnum Status { get; set; } = SalaryStatusEnum.Pending;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}