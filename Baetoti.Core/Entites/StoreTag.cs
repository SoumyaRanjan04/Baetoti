using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace Baetoti.Core.Entites
{
    [Table("StoreTag", Schema = "baetoti")]
    public partial class StoreTag : BaseEntity
    {
        public long StoreID { get; set; }
        public long TagID { get; set; }
    }
}
