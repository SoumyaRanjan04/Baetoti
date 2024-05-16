using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("RolePrivilege", Schema = "baetoti")]
    public partial class RolePrivilege : BaseEntity
    {
        public long RoleID { get; set; }

        public long MenuID { get; set; }

        public long SubMenuID { get; set; }

        public long PrivilegeID { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public string Menu { get; set; }

    }
}
