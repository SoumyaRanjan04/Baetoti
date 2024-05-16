using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.ReportedOrder
{
	public class ReportedOrderByIDResponse
	{
		public long ID { get; set; }

		public string StoreName { get; set; }

		public decimal Price { get; set; }

		public List<ReportedOrderItemResponse> Items { get; set; }

		public DateTime? OrderDate { get; set; }

		public long OrderID { get; set; }

		public ReportedOrderByIDResponse()
		{
			Items = new List<ReportedOrderItemResponse>();
		}

	}
}
