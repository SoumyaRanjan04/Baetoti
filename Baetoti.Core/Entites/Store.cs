using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("Store", Schema = "baetoti")]
    public partial class Store : BaseEntity
    {
        public long ProviderID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public bool IsAddressHidden { get; set; }
        public string VideoURL { get; set; }
        public int Status { get; set; }
        public string BusinessLogo { get; set; }
        public string InstagramGallery { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public bool MarkAsDeleted { get; set; }
        public string InstagramURL { get; set; }
        public string FacebookURL { get; set; }
        public string TikTokURL { get; set; }
        public string TwitterURL { get; set; }
    }
}
