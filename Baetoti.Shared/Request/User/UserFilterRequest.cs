using Baetoti.Shared.Request.Order;

namespace Baetoti.Shared.Request.User
{
    public class UserFilterRequest
    {
        public int UserType { get; set; }
        public int? UserStatus { get; set; }
        public long CountryId { get; set; }
        public string RegionId { get; set; }
        public string CityId { get; set; }
        public string Gender { get; set; }
        public bool? Status { get; set; }
        public DateRangeFilter DateRange { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
