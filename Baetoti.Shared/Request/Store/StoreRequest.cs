using Baetoti.Shared.Request.Shared;
using System.Collections.Generic;

namespace Baetoti.Shared.Request.Store
{
	public class StoreRequest
	{
		public long ID { get; set; }
		public long ProviderID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Location { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public bool IsAddressHidden { get; set; }
		public string VideoURL { get; set; }
		public int Status { get; set; }
		public List<ImageRequest> Images { get; set; }
		public string BusinessLogo { get; set; }
		public string InstagramGallery { get; set; }
        public string InstagramURL { get; set; }
        public string FacebookURL { get; set; }
        public string TikTokURL { get; set; }
        public string TwitterURL { get; set; }
        public List<StoreTagRequest> Tags { get; set; }

		public bool isAddressChanged { get; set; }
		public StoreRequest()
		{
			Tags = new List<StoreTagRequest>();
			Images = new List<ImageRequest>();
		}
	}
	public class StoreTagRequest
	{
		public long ID { get; set; }

		public string Name { get; set; }
	}
}
