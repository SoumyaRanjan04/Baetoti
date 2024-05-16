using System;

namespace Baetoti.Shared.Request.Order
{
	public class OrderFilterRequest
	{
		public long CountryID { get; set; }
		public string RegionID { get; set; }
		public string CityID { get; set; }
		public string Gender { get; set; }
		public long CategoryID { get; set; }
		public long SubCategoryID { get; set; }
		public int? OrderStatus { get; set; }
		public DateRangeFilter DateRange { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

	public class DateRangeFilter
	{
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}
}
