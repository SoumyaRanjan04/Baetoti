using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("SMSTemplate", Schema = "baetoti")]
    public partial class SMSTemplate : BaseEntity
    {
        public int SMSType { get; set; }

        public string SMSText { get; set; }

    }
}
