using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.StoreSchedule
{
    public class StoreScheduleResponse
    {
        public long ID { get; set; }
        public long StoreID { get; set; }
        public string Day { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}
