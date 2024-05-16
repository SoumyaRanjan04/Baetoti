using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("ReportedStore", Schema = "baetoti")]
	public partial class ReportedStore : BaseEntity
	{
		public long StoreID { get; set; }

		public long UserID { get; set; }

		public string Comments { get; set; }

		public DateTime RecordDateTime { get; set; }

	}
}
