using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("FavouriteStore", Schema = "baetoti")]
	public partial class FavouriteStore : BaseEntity
	{
		public long StoreID { get; set; }

		public long UserID { get; set; }

		public DateTime RecordDateTime { get; set; }

	}
}
