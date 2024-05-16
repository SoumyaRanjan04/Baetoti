using Baetoti.Core.Entites.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("SiteConfig", Schema = "baetoti")]
    public partial class SiteConfig : BaseEntity
    {
        public string GovtAPIURL { get; set; }
        public string GovtAPIPassword { get; set; }
        public string GovtAPICompanyName { get; set; }
        public string ChatAPIURL { get; set; }
        public string InstagramAPIURL { get; set; }
        public string NotificationAPIURL { get; set; }
        public string PaymentAPIURL { get; set; }
        public string UseLocalDataForGovtAPIs { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DefaultValue(false)]
        public bool IsSMSEnabled { get; set; }
    }
}
