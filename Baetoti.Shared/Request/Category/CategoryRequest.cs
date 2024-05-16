using System;
using System.ComponentModel.DataAnnotations;

namespace Baetoti.Shared.Request.Category
{
    public class CategoryRequest
    {
        public long ID { get; set; }

        [Required(ErrorMessage ="Please Enter Category Name")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Please Enter Category Arabic Name")]
        public string CategoryArabicName { get; set; }

        [Required(ErrorMessage = "Please Select Color")]
        public string Color { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }
    }
}
