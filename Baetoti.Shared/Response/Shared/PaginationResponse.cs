namespace Baetoti.Shared.Response.Shared
{
	public class PaginationResponse
	{
		public int CurrentPage { get; set; }

		public int TotalPages { get; set; }

		public int PageSize { get; set; }

		public int TotalCount { get; set; }

		public bool HasPrevious => CurrentPage > 1;

		public bool HasNext => CurrentPage < TotalPages;

		public object Data { get; set; }

	}

}
