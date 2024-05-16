namespace Baetoti.Shared.Request.SupportRequest
{
	public class SupportRequestFilter
	{
		public long UserID { get; set; }
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public int SupportRequestStatus { get; set; }
		public string SearchValue { get; set; }
	}
}
