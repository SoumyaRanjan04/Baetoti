using System;

namespace Baetoti.Shared.Response.OrderRequest
{
	public class OrderRequestResponse
	{
		public long OrderID { get; set; }
		public DateTime? DeliveryDate { get; set; }
		public double StoreLongitude { get; set; }
		public double StoreLatitude { get; set; }
		public string StoreAddress { get; set; }
		public double OrderAddressLongitude { get; set; }
		public double OrderAddressLatitude { get; set; }
		public string OrderAddress { get; set; }
		public decimal DeliveryCharges { get; set; }
		public decimal TotalDriverCharges { get; set; }
		public decimal ServiceFee { get; set; }
		public int OrderStatus { get; set; }
		public double Distance { get; set; }
	}
}
