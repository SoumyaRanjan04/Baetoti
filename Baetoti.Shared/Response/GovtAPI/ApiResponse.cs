namespace Baetoti.Shared.Response.GovtAPI
{
	public class ApiResponse<T>
	{
		public bool Status { get; set; }
		public T Data { get; set; }
		public T ErrorCodes { get; set; }
	}
}
