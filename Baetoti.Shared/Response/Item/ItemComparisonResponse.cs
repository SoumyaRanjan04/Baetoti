using Baetoti.Shared.Response.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.Item
{
    public class ItemComparisonResponse
    {
        public Before Before { get; set; }
        public After After { get; set; }
    }

    public class Before
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public decimal Price { get; set; }
        public decimal BaetotiPrice { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public List<TagResponse> Tags { get; set; }
        public List<ImageResponse> Images { get; set; }
    }

    public class After
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public decimal Price { get; set; }
        public decimal BaetotiPrice { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public List<TagResponse> Tags { get; set; }
        public List<ImageResponse> Images { get; set; }
    }

}
