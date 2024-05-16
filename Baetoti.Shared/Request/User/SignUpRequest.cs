namespace Baetoti.Shared.Request.User
{
    public class SignUpRequest
    {
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string CountryCode { get; set; }
        public string PhoneWithCountryCode { get; set; }
        public string MacAddress { get; set; }
    }
}
