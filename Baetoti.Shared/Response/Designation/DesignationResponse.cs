using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.Designation
{
    public class DesignationResponse
    {
        public long ID { get; set; }

        public string DesignationName { get; set; }

        public string DesignationArabicName { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }
    }
}
