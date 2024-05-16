using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Baetoti.Shared.Request.SubCategory
{
    public class SubCategoryRequest
    {
        public long ID { get; set; }

        [Required(ErrorMessage = "Please Select Category")]
        public long CategoryID { get; set; }

        [Required(ErrorMessage = "Please Enter SubCategory Name")]
        public string SubCategoryName { get; set; }

        [Required(ErrorMessage = "Please Enter SubCategory Arabic Name")]
        public string SubCategoryArabicName { get; set; }

        public string Picture { get; set; }
    }
}
