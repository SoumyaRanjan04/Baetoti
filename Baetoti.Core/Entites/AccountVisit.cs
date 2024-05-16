using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("AccountVisit", Schema = "baetoti")]
	public partial class AccountVisit : BaseEntity
	{
		public long ProviderID { get; set; }

		public long DriverID { get; set; }

		public long UserID { get; set; }

		public DateTime FirstVisitDate { get; set; }

		public int UserType { get; set; }

	}
}
