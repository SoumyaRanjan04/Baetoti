using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("ItemReview", Schema = "baetoti")]
    public partial class ItemReview : BaseEntity
    {
		public long OrderID { get; set; }

		public long ItemID { get; set; }

        public long UserID { get; set; }

        public decimal Rating { get; set; }

        public string Review { get; set; }

        public string Image { get; set; }

        public DateTime RecordDateTime { get; set; }

        public bool MarkAsDeleted { get; set; }

    }
}
