namespace Baetoti.Shared.Request.ReportedOrder
{
	public class ReportedOrderRequest
	{
		public long UserID { get; set; }

		public long DriverID { get; set; }

		public long ProviderID { get; set; }

		public long OrderID { get; set; }

		public string Title { get; set; }

		public string Comments { get; set; }

		public string Picture { get; set; }

		public int ReportedByUserType { get; set; }

	}
}
