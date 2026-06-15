namespace HRemployee.PayLoad.DTO
{
    public class DTO_Contract
    {
        public int Id { get; set; }
        public string ContractCode { get; set; }
        public string ContractType { get; set; }
        public int SalaryPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string? Note { get; set; }
        public DateTime? TerminatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
    }
}