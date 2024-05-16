namespace Baetoti.Shared.Request.User
{
    public class OTPRequest
    {
        public long UserID { get; set; }

        public string OTP { get; set; }

        public string MacAddress { get; set; }
    }
}
