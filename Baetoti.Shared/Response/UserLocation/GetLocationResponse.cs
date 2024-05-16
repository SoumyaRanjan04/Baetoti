namespace Baetoti.Shared.Response.UserLocation
{
	public class GetLocationResponse
	{
		public long LocationID { get; set; }

		public long UserID { get; set; }

		public double Latitude { get; set; }

		public string Address { get; set; }

		public string Title { get; set; }

		public double Longitude { get; set; }

		public bool IsSelected { get; set; }

	}
}
