using System;

namespace Baetoti.Shared.Request.Analytic
{
    public class FilterRequest
    {
        public int UserType { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public long CategoryID { get; set; }
        public long SubCategoryID { get; set; }
        public int OrderStatus { get; set; }
        public string RegionID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
