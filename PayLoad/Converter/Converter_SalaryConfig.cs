using HRemployee.Entities;
using HRemployee.PayLoad.DTO;

namespace HRemployee.PayLoad.Converter
{
    public class Converter_SalaryConfig
    {
        public DTO_SalaryConfig ToDTO(SalaryConfig s) => new DTO_SalaryConfig
        {
            Id = s.Id, BaseSalary = s.BaseSalary, Allowance = s.Allowance, KpiBonus = s.KpiBonus, EffectiveDate = s.EffectiveDate, IsActive = s.IsActive, Note = s.Note, CreatedAt = s.CreatedAt, EmployeeId = s.EmployeeId, EmployeeName = s.Employee?.FullName ?? "", EmployeeCode = s.Employee?.EmployeeCode ?? "" };
    }
}