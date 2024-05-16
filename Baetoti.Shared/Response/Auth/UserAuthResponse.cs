using System;

namespace Baetoti.Shared.Response.Auth
{
	public class UserAuthResponse
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public string ExpiresAt { get; set; }
		public UserInformation UserInformation { get; set; }
	}

	public class UserInformation
	{
		public long ID { get; set; }
		public long ProviderID { get; set; }
		public long DriverID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Gender { get; set; }
		public string Description { get; set; }
		public string Email { get; set; }
		public string Picture { get; set; }
		public DateTime? DOB { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }
		public string Country { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }
		public bool IsProfileCompleted { get; set; }
	}

}
