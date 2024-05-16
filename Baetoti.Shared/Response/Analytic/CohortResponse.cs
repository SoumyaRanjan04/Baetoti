using System.Collections.Generic;

namespace Baetoti.Shared.Response.Analytic
{
    public class CohortResponse
    {
        public string MonthLabel { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int SignUps { get; set; }
        public int SignIns { get; set; }
        public List<string> CohortValue { get; set; }

    }
}
