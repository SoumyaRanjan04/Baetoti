namespace Baetoti.Shared.Response.Driver
{
	public class GetAllDriverResponse
	{
		public long ID { get; set; }

		public long UserID { get; set; }

		public string Name { get; set; }

		public string Location { get; set; }

		public string Picture { get; set; }

		public double Longitude { get; set; }

		public double Latitude { get; set; }

		public double Distance { get; set; }

	}
}
