using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Shared
{
	public class RatingAndReviewResponse
	{
		public List<RatingAndReviewResponseData> Data { get; set; }

		public decimal AverageRating { get; set; }

		public int RatingCount { get; set; }

		public RatingAndReviewResponse()
		{
			Data = new List<RatingAndReviewResponseData>();
		}

	}

	public class RatingAndReviewResponseData
	{
		public long OrderID { get; set; }

		public long DriverID { get; set; }

		public long ProviderID { get; set; }

		public long UserID { get; set; }

		public decimal Rating { get; set; }

		public string Reviews { get; set; }

		public DateTime RecordDateTime { get; set; }

		public string ReviewerName { get; set; }

		public string ReviewerPicture { get; set; }

	}
}
