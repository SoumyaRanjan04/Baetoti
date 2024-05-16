namespace Baetoti.Shared.Request.OrderRequest
{
	public class OrderRequestApprovalRequest
	{
		public int OrderRequestStatus { get; set; }

		public long DriverID { get; set; }

		public long OrderID { get; set; }

        public string Comments { get; set; }
    }
}
