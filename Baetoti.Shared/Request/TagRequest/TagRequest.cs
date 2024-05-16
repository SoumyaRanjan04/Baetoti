using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Request.TagRequest
{
    public class TagRequest
    {
        public long ID { get; set; }

		public int TagType { get; set; }

		public int CategoryID { get; set; }

        public int SubCategoryID { get; set; }

        public string TagEnglish { get; set; }

        public string TagArabic { get; set; }
    }
}
