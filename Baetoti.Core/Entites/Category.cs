using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("Categories", Schema = "baetoti")]
    public partial class Category : BaseEntity
    {
        public string CategoryName { get; set; }

        public string CategoryArabicName { get; set; }

        public string Color { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public bool MarkAsDeleted { get; set; }

        public int CategoryStatus { get; set; }

    }
}
