using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("Item", Schema = "baetoti")]
	public partial class Item : BaseEntity
	{
		public string Name { get; set; }

		public string ArabicName { get; set; }

		public string Description { get; set; }

		public int Rating { get; set; }

		public long CategoryID { get; set; }

		public long SubCategoryID { get; set; }

		public long UnitID { get; set; }

		public long ProviderID { get; set; }

		public decimal Price { get; set; }

		public decimal BaetotiPrice { get; set; }

		public int ItemStatus { get; set; }

		public string AveragePreparationTime { get; set; }

		public int AvailableQuantity { get; set; }

		public int? CreatedBy { get; set; }

		public int? UpdatedBy { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? LastUpdatedAt { get; set; }

		public bool MarkAsDeleted { get; set; }

		public int RequestClosedBy { get; set; }

		public DateTime RequestDate { get; set; }

		public DateTime? RequestCloseDate { get; set; }

        public bool IsCheckQuantity { get; set; }

    }
}
