using Baetoti.Shared.Request.Order;

namespace Baetoti.Shared.Request.Transaction
{
    public class TransactionFilterRequest
    {
        public string RegionID { get; set; }
        public string CityID { get; set; }
        public string Gender { get; set; }
        public long CategoryID { get; set; }
        public long CountryID { get; set; }
        public long SubCategoryID { get; set; }
        public string PaymentType { get; set; }
        public int? OrderStatus { get; set; }
        public DateRangeFilter DateRange { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
