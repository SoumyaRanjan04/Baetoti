using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("OperationalConfig", Schema = "baetoti")]
    public partial class OperationalConfig : BaseEntity
    {
        public string Name { get; set; }
        public string Notes { get; set; }
        public int FenceStatus { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public DateTime? StoppedAt { get; set; }
        public bool MarkAsDeleted { get; set; }
    }
}
