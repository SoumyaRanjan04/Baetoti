using Baetoti.Core.Entites.Base;
using System.Buffers.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("Invoice", Schema = "baetoti")]
	public partial class Invoice : BaseEntity
	{
        public long OrderId { get; set; }

        public int InvoiceType { get; set; }

        public string QRCode { get; set; }

    }
}
