namespace Baetoti.Shared.Request.Shared
{
	public class RatingAndReviews
	{
		public int UserType { get; set; }

		public long OrderID { get; set; }

		public long DriverID { get; set; }

		public long ProviderID { get; set; }

		public long UserID { get; set; }

		public decimal Rating { get; set; }

		public string Reviews { get; set; }

	}
}
