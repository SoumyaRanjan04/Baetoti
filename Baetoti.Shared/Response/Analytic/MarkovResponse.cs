using System.Collections.Generic;

namespace Baetoti.Shared.Response.Analytic
{
    public class MarkovResponse
    {
        public string Name { get; set; }

        public List<AnalysisData> Data { get; set; }

        public MarkovResponse()
        {
            Data = new List<AnalysisData>();
        }
    }

    public class AnalysisData
    {
        public string Store { get; set; }

        public decimal Rate { get; set; }

    }
}
