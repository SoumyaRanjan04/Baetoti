using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Request.Item
{
	public class ItemFilterRequest
	{
		public string Name { get; set; }

		public string ArabicName { get; set; }

		public decimal? Price { get; set; }

		public long? CategoryID { get; set; }

		public long? SubCategoryID { get; set; }

		public long? UnitID { get; set; }

		public long? TagID { get; set; }

		public LocationFilter? Location { get; set; }

		public int? LocationRange { get; set; }

		public bool? IsOnlineProvider { get; set; }

		public int? MinimumPrice { get; set; }

		public int? MaximumPrice { get; set; }

		public long? StoreID { get; set; }

		public int PageNumber { get; set; }

		public int PageSize { get; set; }

		public int itemFilter { get; set; } = 0;

	}

	public class LocationFilter
	{
		public decimal Longitude { get; set; }

		public decimal Latitude { get; set; }

	}
}
