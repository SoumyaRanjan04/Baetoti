using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("Region", Schema = "baetoti")]
    public partial class Region
    {
        public string Id { get; set; }

        public string NameAr { get; set; }

        public string NameEn { get; set; }

    }
}
