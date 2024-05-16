using Baetoti.Shared.Response.Shared;
using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Store
{
	public class StoreReviewsAndRatingsResponse
	{
		public decimal AverageRating { get; set; }

		public int RatingCount { get; set; }

		public List<RatingAndReviews> StoreRatingAndReviews { get; set; }

		public StoreReviewsAndRatingsResponse()
		{
			StoreRatingAndReviews = new List<RatingAndReviews>();
		}
	}

	public class RatingAndReviews
	{
		public decimal Rating { get; set; }

		public string Reviews { get; set; }

		public DateTime RecordDateTime { get; set; }

		public long OrderID { get; set; }

		public string ReviewerName { get; set; }

		public string ReviewerPicture { get; set; }

		public string StoreName { get; set; }

		public string StoreLogo { get; set; }

		public List<ItemReviewList> items { get; set; }

		public RatingAndReviews()
		{
			items = new List<ItemReviewList>();
		}

	}

	public class ItemReviewList
	{
		public long ItemID { get; set; }

		public string ItemName { get; set; }

		public string ItemNameArabic { get; set; }

		public decimal Rating { get; set; }

		public string Reviews { get; set; }

		public string ReviewImage { get; set; }

		public string ItemCategory { get; set; }

		public string ItemCategoryArabic { get; set; }

		public DateTime RecordDateTime { get; set; }

		public List<ImageResponse> ItemImages { get; set; }

		public ItemReviewList()
		{
			ItemImages = new List<ImageResponse>();
		}

	}
}
