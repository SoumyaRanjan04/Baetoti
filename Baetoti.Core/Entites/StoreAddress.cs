using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace Baetoti.Core.Entites
{
    [Table("StoreAddress", Schema = "baetoti")]
   public class StoreAddress : BaseEntity
    {
        public long StoreID { get; set; }
        public string Location { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
}
