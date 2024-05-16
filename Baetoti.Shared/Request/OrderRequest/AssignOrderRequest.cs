namespace Baetoti.Shared.Request.OrderRequest
{
	public class AssignOrderRequest
	{
		public long OrderID { get; set; }

		public long DriverID { get; set; }

		public long StoreID { get; set; }

		public double LocationRange { get; set; } = 10;

	}
}
