using Baetoti.Core.Entites.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Baetoti.Core.Entites
{
    [Table("SubCategories", Schema = "baetoti")]
    public partial class SubCategory : BaseEntity
    {
        public int CategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public string SubCategoryArabicName { get; set; }

        public string Picture { get; set; }
        
        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public bool MarkAsDeleted { get; set; }

        public int SubCategoryStatus { get; set; }

    }
}
