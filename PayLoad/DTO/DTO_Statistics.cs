namespace HRemployee.PayLoad.DTO
{
    public class DTO_StatDepartment
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public int ContractActive { get; set; }
    }

    public class DTO_StatEmployee
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public int TotalContracts { get; set; }
        public int ActiveContracts { get; set; }
        public List<DTO_StatDepartment> ByDepartment { get; set; }
    }

    public class DTO_StatAttendanceEmployee
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int PresentDays { get; set; }
        public int LateDays { get; set; }
        public int AbsentDays { get; set; }
        public int LeaveDays { get; set; }
        public decimal TotalWorkingDays { get; set; }
        public decimal TotalWorkingHours { get; set; }
    }

    public class DTO_StatAttendance
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int StandardDays { get; set; }
        public int TotalEmployees { get; set; }
        public int FullyPresentCount { get; set; }
        public int HasAbsentCount { get; set; }
        public List<DTO_StatAttendanceEmployee> Details { get; set; }
    }

    public class DTO_StatSalary
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalEmployees { get; set; }
        public decimal TotalGrossSalary { get; set; }
        public decimal TotalInsurance { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalNetSalary { get; set; }
        public decimal AverageNetSalary { get; set; }
        public decimal MaxNetSalary { get; set; }
        public decimal MinNetSalary { get; set; }
    }
}