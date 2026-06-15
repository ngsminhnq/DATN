using HRemployee.Entities;
using HRemployee.PayLoad.DTO;

namespace HRemployee.PayLoad.Converter
{
    public class Converter_Department
    {
        public DTO_Department ToDTO(Department dept, int employeeCount = 0)
        {
            if (dept == null) return null;
            return new DTO_Department
            {
                Id = dept.Id, Name = dept.Name, Code = dept.Code, Description = dept.Description, EmployeeCount = employeeCount };
        }
    }
}