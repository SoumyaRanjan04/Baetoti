using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Request.Order
{
    public class OrderRequest
    {
        public long UserID { get; set; }
        public string NotesForDriver { get; set; }
        public string DeliveryAddress { get; set; }
        public double AddressLongitude { get; set; }
        public double AddressLatitude { get; set; }
        public DateTime ExpectedDeliveryTime { get; set; }
        public List<RequestedItemList> Items { get; set; }
        public bool IsPickBySelf { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal DriverCommision { get; set; }
        public decimal ProviderCommision { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal Discount { get; set; }
        public OrderRequest()
        {
            Items = new List<RequestedItemList>();
        }
    }

    public class RequestedItemList
    {
        public long ItemID { get; set; }
        public int Quantity { get; set; }
        public string Comments { get; set; }
    }

}
