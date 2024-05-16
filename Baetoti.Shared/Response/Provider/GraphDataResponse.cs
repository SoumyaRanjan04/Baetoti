using System.Collections.Generic;

namespace Baetoti.Shared.Response.Provider
{
    public class GraphDataResponse
    {
        public string Label { get; set; }
        public GraphDataObject Data { get; set; }
    }

    public class GraphDataObject
    {
        public List<string> Labels { get; set; }
        public List<int> Sales { get; set; }
        public List<int> Views { get; set; }
    }

}
