using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("ProviderOrder", Schema = "baetoti")]
    public partial class ProviderOrder : BaseEntity
    {
        public long OrderID { get; set; }
        public long ProviderID { get; set; }
        public int Status { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
