using System;

namespace Baetoti.Shared.Response.SupportRequest
{
	public class SupportRequestResponse
	{
        public long ID { get; set; }

        public long UserID { get; set; }

        public int SupportRequestType { get; set; }

        public string SupportRequestTypeValue { get; set; }

		public string Title { get; set; }

		public string Comments { get; set; }

		public decimal UserRating { get; set; }

		public DateTime RecordDateTime { get; set; }

		public DateTime OpenDateTime { get; set; }

		public DateTime ResolveDateTime { get; set; }

	}
}
