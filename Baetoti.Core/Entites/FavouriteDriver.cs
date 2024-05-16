using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("FavouriteDriver", Schema = "baetoti")]
	public partial class FavouriteDriver : BaseEntity
	{
		public long DriverID { get; set; }

		public long UserID { get; set; }

		public long ProviderID { get; set; }

		public DateTime RecordDateTime { get; set; }

	}
}
