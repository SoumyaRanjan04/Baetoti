using System.Collections.Generic;

namespace Baetoti.Shared.Request.Shared
{
    public class EarnGraphDataResponse
    {
        public string Label { get; set; }
        public EarnGraphDataObject Data { get; set; }
    }

    public class EarnGraphDataObject
    {
        public List<string> Labels { get; set; }
        public List<decimal> Earning { get; set; }
    }

}
