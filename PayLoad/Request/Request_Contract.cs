using HRemployee.Enums;

namespace HRemployee.PayLoad.Request
{
    public class Request_CreateContract
    {
        public string EmployeeCode { get; set; }
        public ContractTypeEnum ContractType { get; set; }
        public int SalaryPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Note { get; set; }
    }

    public class Request_RenewContract
    {
        public DateTime NewEndDate { get; set; }
        public ContractTypeEnum? NewContractType { get; set; }
        public int? NewSalaryPercent { get; set; }
        public string? Note { get; set; }
    }

    public class Request_TerminateContract
    {
        public string? Reason { get; set; }
    }
}