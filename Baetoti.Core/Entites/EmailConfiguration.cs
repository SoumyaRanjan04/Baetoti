using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("EmailConfiguration", Schema = "baetoti")]
    public partial class EmailConfiguration : BaseEntity
    {

        [StringLength(255)]
        public string MailAddress { get; set; } = null!;

        [StringLength(255)]
        public string MailPass { get; set; } = null!;

        [StringLength(255)]
        public string MailSmtp { get; set; } = null!;

        public int Port { get; set; }

        public bool EnableSsl { get; set; }

        public bool IsBodyHtml { get; set; }

    }
}
