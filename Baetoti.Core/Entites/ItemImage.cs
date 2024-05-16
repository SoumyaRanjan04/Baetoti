using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("ItemImage", Schema = "baetoti")]
	public partial class ItemImage : BaseEntity
	{
        public long ItemID { get; set; }

        public string Image { get; set; }

    }
}
