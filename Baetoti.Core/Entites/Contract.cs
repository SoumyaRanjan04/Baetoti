using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("Contract", Schema = "baetoti")]
    public partial class Contract : BaseEntity
    {
        public string Content { get; set; }

        public int ContractType { get; set; }

    }
}
