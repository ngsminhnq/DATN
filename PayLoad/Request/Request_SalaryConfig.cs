using System.ComponentModel;

namespace HRemployee.PayLoad.Request
{

    public class Request_SalaryConfig
    {

        [DefaultValue(15000000)]
        public decimal BaseSalary { get; set; }

        [DefaultValue(500000)]
        public decimal Allowance { get; set; } = 0;

        [DefaultValue(2000000)]
        public decimal KpiBonus { get; set; } = 0;

        [DefaultValue("01/06/2026")]
        public DateTime EffectiveDate { get; set; }

        [DefaultValue("Điều chỉnh lương tháng 6/2026")]
        public string? Note { get; set; }
    }
}