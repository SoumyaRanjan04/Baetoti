using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("UserPromotion", Schema = "baetoti")]
	public partial class UserPromotion : BaseEntity
	{
		public long UserID { get; set; }

		public long PromotionID { get; set; }

		public long OrderID { get; set; }

		public int? CreatedBy { get; set; }

		public int? UpdatedBy { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? LastUpdatedAt { get; set; }

	}
}
