using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.SubMenu
{
    public class SubMenuResponse
    {
        public long ID { get; set; }

        public long MenuID { get; set; }

        public string MenuName { get; set; }

        public string SubMenuName { get; set; }
    }
}
