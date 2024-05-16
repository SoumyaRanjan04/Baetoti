using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("OrderRequest", Schema = "baetoti")]
	public partial class OrderRequest : BaseEntity
	{
		public long OrderID { get; set; }

		public long StoreID { get; set; }

		public long DriverID { get; set; }

		public int RequestStatus { get; set; }

		public DateTime RequestDateTime { get; set; }

        public string Comments { get; set; }

    }
}
