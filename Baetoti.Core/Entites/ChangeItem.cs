using Baetoti.Core.Entites.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Baetoti.Core.Entites
{
    [Table("ChangeItem", Schema = "baetoti")]
    public class ChangeItem : BaseEntity
    {
        public long ItemId { get; set; }

        public string Name { get; set; }

        public string ArabicName { get; set; }

        public string Description { get; set; }

        public int Rating { get; set; }

        public long CategoryID { get; set; }

        public long SubCategoryID { get; set; }

        public long UnitID { get; set; }

        public long ProviderID { get; set; }

        public decimal Price { get; set; }

		public int AvailableQuantity { get; set; }

		public decimal BaetotiPrice { get; set; }

		public string Picture1 { get; set; }

		public string Picture2 { get; set; }

		public string Picture3 { get; set; }

		public string AveragePreparationTime { get; set; }

        public int ItemStatus { get; set; }

        public DateTime RequestDate { get; set; }
        
        public DateTime? RequestCloseDate { get; set; }

        public long RequestedBy { get; set; }

        public long RequestClosedBy { get; set; }

        public string Comments { get; set; }

		public bool IsCheckQuantity { get; set; }

	}
}
