using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("BuyerReview", Schema = "baetoti")]
	public partial class BuyerReview : BaseEntity
	{
		public long OrderID { get; set; }

		public long ProviderID { get; set; }

		public long DriverID { get; set; }

		public long UserID { get; set; }

		public decimal Rating { get; set; }

		public string Reviews { get; set; }

		public DateTime RecordDateTime { get; set; }

	}
}
