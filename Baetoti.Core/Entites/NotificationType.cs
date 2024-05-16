using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("NotificationType", Schema = "baetoti")]
    public partial class NotificationType : BaseEntity
    {
        public string Type { get; set; }

    }
}
