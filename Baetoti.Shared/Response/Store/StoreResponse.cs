using Baetoti.Shared.Response.Shared;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Store
{
	public class StoreResponse
	{
		public long ID { get; set; }
		public long ProviderID { get; set; }
		public long ProviderUserID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public double Distance { get; set; }
		public string Location { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public bool IsAddressHidden { get; set; }
		public string VideoURL { get; set; }
		public int Status { get; set; }
		public string BusinessLogo { get; set; }
		public string InstagramToken { get; set; }
		public string InstagramGallery { get; set; }
        public string InstagramURL { get; set; }
        public string FacebookURL { get; set; }
        public string TikTokURL { get; set; }
        public string TwitterURL { get; set; }
        public string Category { get; set; }
		public decimal AverageRating { get; set; }
		public List<ImageResponse> Images { get; set; }
		public List<StoreTagResponse> Tags { get; set; }
		public List<RatingAndReviews> RatingAndReviews { get; set; }
		public int TotalRating { get; set; }
		public int TotalFavourites { get; set; }
		public bool IsFavourite { get; set; }
		public bool IsOnline { get; set; }
		public decimal MinimumItemPrice { get; set; }
		public decimal MaximumItemPrice { get; set; }
        public double OrderAcceptanceRate { get; set; }
        public StoreResponse()
		{
			Images = new List<ImageResponse>();
			Tags = new List<StoreTagResponse>();
			RatingAndReviews = new List<RatingAndReviews>();
		}
	}
}