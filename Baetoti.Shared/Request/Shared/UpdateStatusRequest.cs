namespace Baetoti.Shared.Request.Shared
{
	public class UpdateStatusRequest
	{
		public int ID { get; set; }

		public int Value { get; set; }

		public int UserType { get; set; } = 1;

		public string Comments { get; set; }

	}
}
