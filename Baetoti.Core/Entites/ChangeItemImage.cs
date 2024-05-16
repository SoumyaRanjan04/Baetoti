using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("ChangeItemImage", Schema = "baetoti")]
	public partial class ChangeItemImage : BaseEntity
	{
		public long ItemID { get; set; }

		public long ChangeItemID { get; set; }

		public string Image { get; set; }

	}
}
