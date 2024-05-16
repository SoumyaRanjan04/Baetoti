using System;

namespace Baetoti.Shared.Response.User
{
    public class UserResponse
    {
        public UserSummary userSummary { get; set; }
        public ProviderSummary providerSummary { get; set; }
        public DriverSummary driverSummary { get; set; }
    }

    public class UserList
    {
        public long UserID { get; set; }
        public string Name { get; set; }
        public decimal Revenue { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string UserStatus { get; set; }
        public string ProviderStatus { get; set; }
        public string DriverStatus { get; set; }
        public DateTime? SignUpDate { get; set; }
        public bool IsOnline { get; set; }
        public double OrderAcceptanceRate { get; set; }

    }

    public class UserSummary
    {
        public int TotalUser { get; set; }
        public int NewUser { get; set; }
        public int ActiveUser { get; set; }
        public int LiveUser { get; set; }
        public int ReportedUser { get; set; }
    }

    public class ProviderSummary
    {
        public int TotalProvider { get; set; }
        public int NewProvider { get; set; }
        public int ActiveProvider { get; set; }
        public int LiveProvider { get; set; }
        public int ReportedProvider { get; set; }
    }

    public class DriverSummary
    {
        public int TotalDriver { get; set; }
        public int NewDriver { get; set; }
        public int ActiveDriver { get; set; }
        public int LiveDriver { get; set; }
        public int ReportedDriver { get; set; }
    }

}
