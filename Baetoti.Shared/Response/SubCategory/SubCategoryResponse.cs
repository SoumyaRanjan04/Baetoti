using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Baetoti.Shared.Response.SubCategory
{
    public class SubCategoryResponse
    {
        public long ID { get; set; }

        public long CategoryID { get; set; }

        public string CategoryName { get; set; }

        public string CategoryArabicName { get; set; }

        public string SubCategoryName { get; set; }

        public string SubCategoryArabicName { get; set; }

        public string Picture { get; set; }

        public int SubCategoryStatus { get; set; }

    }
}
