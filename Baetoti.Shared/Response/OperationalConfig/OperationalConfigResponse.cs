using System;

namespace Baetoti.Shared.Response.OperationalConfig
{
    public class OperationalConfigResponse
    {
        public long ID { get; set; }

        public string Title { get; set; }

        public string Notes { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public string Status { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public DateTime? StopedAt { get; set; }

        public int TotalUser { get; set; }

        public int TotalProvider { get; set; }

        public int TotalDriver { get; set; }

    }
}
