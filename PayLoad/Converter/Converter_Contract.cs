using HRemployee.Entities;
using HRemployee.PayLoad.DTO;

namespace HRemployee.PayLoad.Converter
{
    public class Converter_Contract
    {
        public DTO_Contract ToDTO(Contract c)
        {
            if (c == null) return null;
            return new DTO_Contract
            {
                Id = c.Id,
                ContractCode = c.ContractCode,
                ContractType = c.ContractType switch
                {
                    Enums.ContractTypeEnum.HocViec => "Học việc", Enums.ContractTypeEnum.ThuViec => "Thử việc", Enums.ContractTypeEnum.ChinhThuc => "Chính thức", _ => c.ContractType.ToString() }, SalaryPercent = c.SalaryPercent, StartDate = c.StartDate, EndDate = c.EndDate, Status = c.Status.ToString(), Note = c.Note, TerminatedAt = c.TerminatedAt, CreatedAt = c.CreatedAt, EmployeeId = c.EmployeeId, EmployeeName = c.Employee?.FullName ?? "", EmployeeCode = c.Employee?.EmployeeCode ?? "" };
        }
    }
}