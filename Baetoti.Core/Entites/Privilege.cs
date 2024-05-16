using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("Privilege", Schema = "baetoti")]
    public partial class Privilege : BaseEntity
    {
        public string Name { get; set; }
    }
}
