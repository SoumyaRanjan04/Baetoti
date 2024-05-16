using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("StoreImage", Schema = "baetoti")]
	public partial class StoreImage : BaseEntity
	{
		public long StoreID { get; set; }

		public string Image { get; set; }

	}
}
