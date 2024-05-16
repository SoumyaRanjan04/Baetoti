using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("EmailTemplate", Schema = "baetoti")]
    public partial class EmailTemplate : BaseEntity
    {
        public int EmailType { get; set; }

        [StringLength(255)]
        public string Subject { get; set; }

        public string HtmlBody { get; set; }

    }
}
