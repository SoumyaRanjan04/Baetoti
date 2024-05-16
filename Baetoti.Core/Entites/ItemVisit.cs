using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("ItemVisit", Schema = "baetoti")]
    public partial class ItemVisit : BaseEntity
    {
        public long ItemID { get; set; }

        public long UserID { get; set; }

        public DateTime FirstVisitDate { get; set; }

    }
}
