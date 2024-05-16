using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.SupportRequest
{
    public class DashboardResponse
    {
        public DashboardResponse()
        {
            GraphData = new List<CSSGraphData>();
        }

        public int Pending { get; set; }

        public int Open { get; set; }

        public int Resolved { get; set; }

        public decimal CustomerSatisfaction { get; set; }

        public List<CSSGraphData> GraphData { get; set; }

    }

    public class CSSGraphData
    {
        public DateTime Date { get; set; }

        public decimal AverageCustomerSatisfaction { get; set; }

        public decimal AverageResponseTime { get; set; }

        public decimal AverageResolutionTime { get; set; }

    }

}
