namespace HRemployee.PayLoad.DTO
{
    public class DTO_SalaryRecord
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public decimal BaseSalary { get; set; }
        public decimal Allowance { get; set; }
        public decimal KpiBonus { get; set; }
        public decimal KpiCoefficient { get; set; }
        public int SalaryPercent { get; set; }

        public int StandardDays { get; set; }
        public decimal WorkingDays { get; set; }

        public decimal GrossSalary { get; set; }
        public decimal InsuranceDeduction { get; set; }
        public decimal TaxDeduction { get; set; }
        public decimal NetSalary { get; set; }

        public string Status { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string? EmployeeEmail { get; set; }
    }
}