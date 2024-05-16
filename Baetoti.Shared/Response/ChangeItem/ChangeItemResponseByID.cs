using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.ChangeItem
{
	public class ChangeItemResponseByID
	{
		public long ID { get; set; }
		public long ItemId { get; set; }
		public string StoreName { get; set; }
		public string Location { get; set; }
		public string Title { get; set; }
		public long CategoryID { get; set; }
		public string Category { get; set; }
		public long SubCategoryID { get; set; }
		public string SubCategory { get; set; }
		public string Price { get; set; }
		public long UnitID { get; set; }
		public string Unit { get; set; }
		public string Picture1 { get; set; }
		public string Picture2 { get; set; }
		public string Picture3 { get; set; }
		public long Sold { get; set; }
		public long AvailableNow { get; set; }
		public long Quantity { get; set; }
		public long TotalRevenue { get; set; }
		public string AveragePreparationTime { get; set; }
		public string Description { get; set; }
		public List<ChanngeItemTagResponse> Tags { get; set; }
		public List<ChangeItemReviewResponse> Reviews { get; set; }
		public List<ChangeRecentOrder> RecentOrder { get; set; }
		public decimal AverageRating { get; set; }
	}

	public class ChangeRecentOrder
	{
		public int OrderID { get; set; }
		public string Driver { get; set; }
		public string Buyer { get; set; }
		public DateTime PickUp { get; set; }
		public DateTime Delivery { get; set; }
		public decimal Rating { get; set; }
		public string Reviews { get; set; }
	}

	public class ChangeItemReviewResponse
	{
		public string UserName { get; set; }
		public string Picture { get; set; }
		public decimal Rating { get; set; }
		public string Reviews { get; set; }
		public DateTime ReviewDate { get; set; }
	}

	public class ChanngeItemTagResponse
	{
		public long ID { get; set; }
		public string Name { get; set; }
	}
}
