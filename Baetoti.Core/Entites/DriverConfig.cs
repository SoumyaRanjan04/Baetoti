using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("DriverConfig", Schema = "baetoti")]
	public partial class DriverConfig : BaseEntity
	{
		public decimal FromKM { get; set; }
		public decimal ToKM { get; set; }
		public decimal RatePerKM { get; set; }
		public decimal AdditionalRatePerKM { get; set; }
		public string Currency { get; set; }
		public decimal AdditionalKM { get; set; }
		public decimal MaximumDistance { get; set; }
		public decimal DriverComission { get; set; }
		public decimal ProviderComission { get; set; }
		public decimal ServiceFee { get; set; }
		public decimal ServiceFeeFixed { get; set; }
		public int? CreatedBy { get; set; }
		public int? UpdatedBy { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? LastUpdatedAt { get; set; }
		public bool MarkAsDeleted { get; set; }
	}
}
