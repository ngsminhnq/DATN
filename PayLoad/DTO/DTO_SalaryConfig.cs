namespace HRemployee.PayLoad.DTO
{
    public class DTO_SalaryConfig
    {
        public int Id { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Allowance { get; set; }
        public decimal KpiBonus { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
    }
}