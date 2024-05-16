using System.Collections.Generic;

namespace Baetoti.Shared.Response.Analytic
{
    public class RevenuePerDayResponse
    {
        public string Label { get; set; }
        public Dictionary<string, object> Revenue { get; set; }
    }

}
