using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.Unit
{
    public class UnitResponse
    {
        public long ID { get; set; }

        public string UnitType { get; set; }

        public string UnitArabicName { get; set; }

        public string UnitEnglishName { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public int UnitStatus { get; set; }

    }
}
