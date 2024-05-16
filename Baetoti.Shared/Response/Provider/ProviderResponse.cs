using System;

namespace Baetoti.Shared.Response.Provider
{
	public class ProviderResponse
	{
		public long UserID { get; set; }
		public long ProviderID { get; set; }
		public string MaroofID { get; set; }
		public string MaroofLink { get; set; }
		public string GovernmentID { get; set; }
		public string GovernmentIDPicture { get; set; }
		public DateTime ExpirationDate { get; set; }
		public DateTime? CreatedAt { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string ProfileImage { get; set; }
		public bool IsPublic { get; set; }
		public bool IsOnline { get; set; }
	}
}
