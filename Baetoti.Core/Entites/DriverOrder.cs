using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("DriverOrder", Schema = "baetoti")]
    public partial class DriverOrder : BaseEntity
    {
        public long OrderID { get; set; }
        public long DriverID { get; set; }
        public int Status { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string Comments { get; set; }
    }
}
