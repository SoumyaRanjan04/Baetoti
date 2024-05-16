using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("Fence", Schema = "baetoti")]
    public partial class Fence : BaseEntity
    {
        public string Title { get; set; }

        public string Notes { get; set; }

        public long CountryID { get; set; }

        public string Region { get; set; }

        public string RegionID { get; set; }

        public string City { get; set; }

        public string CityID { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public int? StopedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public DateTime? StopedAt { get; set; }

        public int FenceStatus { get; set; }

    }
}
