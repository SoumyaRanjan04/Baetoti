using Baetoti.Core.Entites.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Baetoti.Core.Entites
{
    [Table("Units", Schema = "baetoti")]
    public partial class Unit : BaseEntity
    {

        public string UnitType { get; set; }

        public string UnitArabicName { get; set; }

        public string UnitEnglishName { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public bool MarkAsDeleted { get; set; }

        public int UnitStatus { get; set; }

    }
}
