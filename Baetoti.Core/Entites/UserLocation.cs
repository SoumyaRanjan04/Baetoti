using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("UserLocation", Schema = "baetoti")]
	public partial class UserLocation : BaseEntity
	{
		public long UserID { get; set; }

		public double Latitude { get; set; }

		public string Address { get; set; }

		public string Title { get; set; }

		public double Longitude { get; set; }

		public int? CreatedBy { get; set; }

		public int? UpdatedBy { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? LastUpdatedAt { get; set; }

		public bool MarkAsDeleted { get; set; }

		public bool IsDefault { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[DefaultValue(false)]
		public bool IsLive { get; set; }

	}
}
