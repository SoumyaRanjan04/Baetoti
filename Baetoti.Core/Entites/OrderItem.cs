using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("OrderItem", Schema = "baetoti")]
    public partial class OrderItem : BaseEntity
    {
        public long OrderID { get; set; }
        public long ItemID { get; set; }
        public int Quantity { get; set; }
        public decimal SoldPrice { get; set; }
        public string Comments { get; set; }
    }
}
