using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Request.Item
{
    public class ItemRequestApprovel
    {
        public long ID { get; set; }

        public long ItemID { get; set; }

        public int StatusValue { get; set; }

        public string Comments { get; set; }

        public int ItemRequestType { get; set; }

    }
}
