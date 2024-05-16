using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("Order", Schema = "baetoti")]
    public partial class Order : BaseEntity
    {
        public long UserID { get; set; }
        public string NotesForDriver { get; set; }
        public string ProviderComments { get; set; }
        public string DriverComments { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime ExpectedDeliveryTime { get; set; }
        public DateTime ActualDeliveryTime { get; set; }
        public DateTime OrderReadyTime { get; set; }
        public DateTime OrderPickUpTime{ get; set; }
		public double AddressLongitude { get; set; }
		public double AddressLatitude { get; set; }
		public bool IsPickBySelf { get; set; }
		public int Status { get; set; }
        public decimal Rating { get; set; }
        public string Review { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
