using HRemployee.Entities;
using HRemployee.PayLoad.DTO;

namespace HRemployee.PayLoad.Converter
{
    public class Converter_Position
    {
        public DTO_Position ToDTO(Position pos, int employeeCount = 0)
        {
            if (pos == null) return null;
            return new DTO_Position
            {
                Id = pos.Id, Name = pos.Name, Code = pos.Code, Description = pos.Description, EmployeeCount = employeeCount };
        }
    }
}