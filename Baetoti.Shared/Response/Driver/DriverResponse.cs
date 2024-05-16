using System;

namespace Baetoti.Shared.Response.Driver
{
    public class DriverResponse
    {
        public long UserID { get; set; }
        public string IDExpiryDate { get; set; }
        public string FrontSideofIDPic { get; set; }
        public string PersonalPic { get; set; }
        public string DrivingLicensePic { get; set; }
        public string ExpirayDateofLicense { get; set; }
        public string VehicleRegistrationPic { get; set; }
        public string VehicleAuthorizationPic { get; set; }
        public string ExpiryDateofVehicleAuthorization { get; set; }
        public string MedicalCheckupPic { get; set; }
        public string ExpiryDateofMedicalcheckup { get; set; }
        public string VehicleInsurancePic { get; set; }
        public string ExpiryDateofVehicleInsurance { get; set; }
        public DateTime? CreatedAt { get; set; }
		public string IdentityTypeId { get; set; }
		public string CarTypeId { get; set; }
		public string RegionId { get; set; }
		public string CityId { get; set; }
		public DateTime RegistrationDate { get; set; }
		public string MobileNumber { get; set; }
		public string CarNumber { get; set; }
		public string VehicleSequenceNumber { get; set; }
        public string Nationality { get; set; }
        public string DOB { get; set; }
        public string IDNumber { get; set; }
    }
}
