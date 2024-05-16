using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("StoreReview", Schema = "baetoti")]
	public partial class StoreReview : BaseEntity
	{
		public long OrderID { get; set; }

		public long StoreID { get; set; }

		public long UserID { get; set; }

		public long DriverID { get; set; }

		public decimal Rating { get; set; }

		public string Reviews { get; set; }

		public DateTime RecordDateTime { get; set; }

	}
}
