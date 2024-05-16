using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Request.UnitRequest
{
    public class UnitRequest
    {
        public long ID { get; set; }

        public string UnitType { get; set; }

        public string UnitArabicName { get; set; }

        public string UnitEnglishName { get; set; }
    }
}
