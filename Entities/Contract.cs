using HRemployee.Enums;

namespace HRemployee.Entities
{
    public class Contract : EntityBase
    {
        public string ContractCode { get; set; }
        public ContractTypeEnum ContractType { get; set; }
        public int SalaryPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatusEnum Status { get; set; } = ContractStatusEnum.Active;
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? TerminatedAt { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}