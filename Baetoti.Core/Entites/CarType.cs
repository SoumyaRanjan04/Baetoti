using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("CarType", Schema = "baetoti")]
    public partial class CarType
    {
        public string Id { get; set; }

        public string NameAr { get; set; }

        public string NameEn { get; set; }

    }
}
