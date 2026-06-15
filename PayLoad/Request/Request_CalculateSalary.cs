using System.ComponentModel;

namespace HRemployee.PayLoad.Request
{

    public class Request_CalculateSalary
    {

        [DefaultValue(1.0)]
        public decimal KpiCoefficient { get; set; } = 1.0m;

        [DefaultValue("Lương tháng 5/2026")]
        public string? Note { get; set; }
    }
}