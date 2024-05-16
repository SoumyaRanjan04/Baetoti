namespace Baetoti.Shared.Request.OrderRequest
{
	public class GetOrderRequest
	{
        public long DriverID { get; set; }

		public int PageNumber { get; set; } = 1;

		public int PageSize { get; set; } = 10;

		public string SearchValue { get; set; }

		public int OrderRequestStatus { get; set; }

	}
}
