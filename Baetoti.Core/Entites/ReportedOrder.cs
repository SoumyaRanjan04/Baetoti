using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("ReportedOrder", Schema = "baetoti")]
	public class ReportedOrder : BaseEntity
	{
		public long UserID { get; set; }

		public long DriverID { get; set; }

		public long ProviderID { get; set; }

		public long OrderID { get; set; }

		public string Comments { get; set; }

		public string Picture { get; set; }

		public int ReportedByUserType { get; set; }

		public DateTime RecordDateTime { get; set; }

	}
}
