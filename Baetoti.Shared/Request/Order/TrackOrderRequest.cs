namespace Baetoti.Shared.Request.Order
{
	public class TrackOrderRequest
	{
		public int UserType { get; set; } = 1;
		public int TrackOrderStatus { get; set; }
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string SearchValue { get; set; }
		public int SortOrder { get; set; } = 1;
	}
}
