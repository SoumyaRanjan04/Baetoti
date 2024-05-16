using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("Menu", Schema = "baetoti")]
    public partial class Menu : BaseEntity
    {
        public string Name { get; set; }

    }
}
