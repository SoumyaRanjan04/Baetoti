using Baetoti.Shared.Response.Shared;
using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Item
{
	public class ItemResponse
	{
		public int Total { get; set; }
		public int Active { get; set; }
		public int Inactive { get; set; }
		public int Flaged { get; set; }
		public List<ItemListResponse> items { get; set; }
	}

	public class ItemListResponse
	{
		public long ID { get; set; }
		public long StoreID { get; set; }
		public string StoreName { get; set; }
		public string StoreLogo { get; set; }
		public List<ImageResponse> StoreImages { get; set; }
		public long ProviderID { get; set; }
		public long ProviderUserID { get; set; }
		public string Title { get; set; }
		public List<ImageResponse> ItemImages { get; set; }
		public string Description { get; set; }
		public long CategoryID { get; set; }
		public string Category { get; set; }
		public string CategoryArabic { get; set; }
		public string CategoryColor { get; set; }
		public string CategoryDescription { get; set; }
		public int CategoryStatus { get; set; }
		public string CategoryPicture { get; set; }
		public long SubCategoryID { get; set; }
		public string SubCategory { get; set; }
		public string SubCategoryArabic { get; set; }
		public string SubCategoryPicture { get; set; }
		public int SubCategoryStatus { get; set; }
		public long OrderedQuantity { get; set; }
		public decimal Revenue { get; set; }
		public string AveragePreparationTime { get; set; }
		public int Status { get; set; }
		public string StatusValue { get; set; }
		public decimal Price { get; set; }
		public decimal BaetotiPrice { get; set; }
		public long UnitID { get; set; }
		public string Unit { get; set; }
		public string UnitArabic { get; set; }
		public string UnitType { get; set; }
		public decimal Rating { get; set; }
		public int RatingCount { get; set; }
		public DateTime? CreateDate { get; set; }
		public List<ItemTagsList> Tags { get; set; }
		public string CategoryImage { get; set; }
		public double Distance { get; set; }
		public int AvailableQuantity { get; set; }
		public int VisitCount { get; set; }
        public bool IsAddedToCart { get; set; }
        public bool IsItemAlreadyViewed { get; set; }
        public ItemListResponse()
		{
			Tags = new List<ItemTagsList>();
			ItemImages = new List<ImageResponse>();
			StoreImages = new List<ImageResponse>();
		}
	}

	public class ItemTagsList
	{
        public long TagID { get; set; }
        public string TagEnglish { get; set; }
		public string TagArabic { get; set; }
	}

}
