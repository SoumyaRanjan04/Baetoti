using System;

namespace Baetoti.Shared.Request.User
{
    public class CompleteProfileRequest
    {
        public long UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public DateTime? DOB { get; set; }
        public bool IsSocialLoginUsed { get; set; }
        public long CountryID { get; set; }
        public string CityID { get; set; }
        public string  RegionID { get; set; }
    }
}
