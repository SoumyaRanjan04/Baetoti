using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Request.Store
{
	public class StoreReviewAndRatingFilterRequest
	{
		public long StoreID { get; set; }

		public int RatingType { get; set; }

	}
}
