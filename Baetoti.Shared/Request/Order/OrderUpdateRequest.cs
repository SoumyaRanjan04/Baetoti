using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Request.Order
{
    public class OrderUpdateRequest
    {
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public long ProviderId { get; set; }
        public long DriverId { get; set; }
        public long OrderId { get; set; }
    }
}
