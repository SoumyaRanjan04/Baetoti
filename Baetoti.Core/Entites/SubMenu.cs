using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("SubMenu", Schema = "baetoti")]
    public partial class SubMenu : BaseEntity
    {
        public long MenuID { get; set; }

        public string Name { get; set; }

    }
}
