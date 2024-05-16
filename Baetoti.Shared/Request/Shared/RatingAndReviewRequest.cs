namespace Baetoti.Shared.Request.Shared
{
	public class RatingAndReviewRequest
	{
		public int UserType { get; set; }

		public int RatingFilter { get; set; }

		public int PageNumber { get; set; } = 1;

		public int PageSize { get; set; } = 10;

	}
}
