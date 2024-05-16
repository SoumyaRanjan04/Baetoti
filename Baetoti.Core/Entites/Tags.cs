using Baetoti.Core.Entites.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Baetoti.Core.Entites
{
	[Table("Tags", Schema = "baetoti")]
	public partial class Tags : BaseEntity
	{
		public int TagType { get; set; }

		public int CategoryID { get; set; }

		public int SubCategoryID { get; set; }

		public string TagEnglish { get; set; }

		public string TagArabic { get; set; }

		public int? CreatedBy { get; set; }

		public int? UpdatedBy { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? LastUpdatedAt { get; set; }

		public bool MarkAsDeleted { get; set; }

		public int TagStatus { get; set; }

	}
}
