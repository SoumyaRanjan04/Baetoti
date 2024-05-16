using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("ItemTag", Schema = "baetoti")]
    public partial class ItemTag : BaseEntity
    {
        public long ItemID { get; set; }

        public long TagID { get; set; }

    }
}
