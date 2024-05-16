using Baetoti.Shared.Response.Shared;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Cart
{
	public class GetCartResponse
	{
		public List<CartItem> CartItems { get; set; }
		public decimal DeliveryCharges { get; set; }
		public decimal TotalCharges { get; set; }
		public decimal DriverCommision { get; set; }
		public decimal ProviderCommision { get; set; }
		public decimal ServiceFee { get; set; }
	}

	public class CartItem
	{
		public long ItemID { get; set; }
		public string ItemName { get; set; }
		public string ItemComments { get; set; }
		public List<ImageResponse> ItemImages { get; set; }
		public long StoreID { get; set; }
		public string StoreName { get; set; }
		public string StoreLogo { get; set; }
		public List<ImageResponse> StoreImages { get; set; }
		public decimal Price { get; set; }
		public decimal BaetotiPrice { get; set; }
		public decimal StoreMinimumItemPrice { get; set; }
		public int Quantity { get; set; }
		public long CategoryID { get; set; }
		public string Category { get; set; }
		public string CategoryArabic { get; set; }
		public CartItem()
		{
			ItemImages = new List<ImageResponse>();
			StoreImages = new List<ImageResponse>();
		}
	}
}
