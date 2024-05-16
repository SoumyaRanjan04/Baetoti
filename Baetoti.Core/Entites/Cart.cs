using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("Cart", Schema = "baetoti")]
	public partial class Cart : BaseEntity
	{
		public long UserID { get; set; }

		public long ItemID { get; set; }

		public int Quantity { get; set; }

		public string Comments { get; set; }

		public DateTime RecordDateTime { get; set; }

	}
}
