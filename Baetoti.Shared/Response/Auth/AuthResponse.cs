using System;

namespace Baetoti.Shared.Response.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiresAt { get; set; }
        public EmployeeInformation UserInformation { get; set; }
    }

    public class EmployeeInformation
    {
        public long UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public DateTime? DOB { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Comments { get; set; }
    }
}
