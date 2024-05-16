using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.Department
{
    public class DepartmentResponse
    {
        public long ID { get; set; }

        public string DepartmentName { get; set; }

        public string DepartmentArabicName { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }
    }
}
