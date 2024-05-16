using System.Collections.Generic;

namespace Baetoti.Shared.Request.Store
{
	public class StoreRatingAndReviewRequest
	{
		public long OrderID { get; set; }

		public long StoreID { get; set; }

		public long UserID { get; set; }

		public long DriverID { get; set; }

		public decimal Rating { get; set; }

		public string Reviews { get; set; }

		public List<ItemReviewRequest> Items { get; set; }

		public StoreRatingAndReviewRequest()
		{
			Items = new List<ItemReviewRequest>();
		}

	}

	public class ItemReviewRequest
	{
		public long ItemID { get; set; }

		public decimal Rating { get; set; }

		public string Reviews { get; set; }

		public string Image { get; set; }

	}

}
