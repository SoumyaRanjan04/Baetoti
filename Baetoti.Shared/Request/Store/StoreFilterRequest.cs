namespace Baetoti.Shared.Request.Store
{
	public class StoreFilterRequest
	{
		public string StoreName { get; set; }

		public int ProviderOnlineStatus { get; set; }

		public double LocationRange { get; set; }

		public decimal MinimumPrice { get; set; }

		public decimal MaximumPrice { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }

	}
}
