using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("City", Schema = "baetoti")]
    public partial class City
    {
        public string Id { get; set; }

        public string RegionId { get; set; }

        public string NameAr { get; set; }

        public string NameEn { get; set; }
    }
}
