using System;

namespace Baetoti.Shared.Response.TagResponse
{
	public class TagResponse
	{
		public long ID { get; set; }

		public int TagType { get; set; }

		public string TagTypeName { get; set; }

		public int? CategoryID { get; set; }

		public string CategoryEnglish { get; set; }

		public string CategoryArabic { get; set; }

		public int? SubCategoryID { get; set; }

		public string SubCategoryEnglish { get; set; }

		public string SubCategoryArabic { get; set; }

		public string TagEnglish { get; set; }

		public string TagArabic { get; set; }

		public int? CreatedBy { get; set; }

		public int? UpdatedBy { get; set; }

		public string CreatedByName { get; set; }

		public string UpdatedByName { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? LastUpdatedAt { get; set; }

		public int TagStatus { get; set; }

	}
}
