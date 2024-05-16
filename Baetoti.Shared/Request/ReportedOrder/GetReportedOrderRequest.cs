namespace Baetoti.Shared.Request.ReportedOrder
{
	public class GetReportedOrderRequest
	{
		public long UserID { get; set; }

		public long DriverID { get; set; }

		public long ProviderID { get; set; }

		public int PageNumber { get; set; } = 1;

		public int PageSize { get; set; } = 10;

		public string SearchValue { get; set; }

		public int ReportedOrderStatus { get; set; }

	}
}
