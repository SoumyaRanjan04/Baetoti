using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("StoreSchedule", Schema = "baetoti")]
    public partial class StoreSchedule : BaseEntity
    {
        public long StoreID { get; set; }

        public string Day { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public bool MarkAsDeleted { get; set; }
    }
}
