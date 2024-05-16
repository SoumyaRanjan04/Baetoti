namespace Baetoti.Shared.Request.Order
{
	public class OrderStatusSearchRequest
	{
		public int UserType { get; set; } = 1;
		public int OrderStatus { get; set; }
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string SearchValue { get; set; }
		public int SortOrder { get; set; } = 1;
	}
}
