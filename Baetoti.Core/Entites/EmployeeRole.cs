using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("UserRoles", Schema = "baetoti")]

    public partial class EmployeeRole : BaseEntity
    {
        public int EmployeeId { get; set; }

        public int RoleId { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public bool MarkAsDeleted { get; set; }
    }
}
