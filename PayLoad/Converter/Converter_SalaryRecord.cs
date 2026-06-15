using HRemployee.Entities;
using HRemployee.PayLoad.DTO;

namespace HRemployee.PayLoad.Converter
{
    public class Converter_SalaryRecord
    {
        public DTO_SalaryRecord ToDTO(SalaryRecord s) => new DTO_SalaryRecord
        {
            Id = s.Id, Month = s.Month, Year = s.Year, BaseSalary = s.BaseSalary, Allowance = s.Allowance, KpiBonus = s.KpiBonus, KpiCoefficient = s.KpiCoefficient, SalaryPercent = s.SalaryPercent, StandardDays = s.StandardDays, WorkingDays = s.WorkingDays, GrossSalary = s.GrossSalary, InsuranceDeduction = s.InsuranceDeduction, TaxDeduction = s.TaxDeduction, NetSalary = s.NetSalary, Status = s.Status.ToString(), Note = s.Note, CreatedAt = s.CreatedAt, EmployeeId = s.EmployeeId, EmployeeName = s.Employee?.FullName ?? "", EmployeeCode = s.Employee?.EmployeeCode ?? "", EmployeeEmail = s.Employee?.Email };
    }
}