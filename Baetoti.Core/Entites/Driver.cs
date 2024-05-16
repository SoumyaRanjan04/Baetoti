using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("Driver", Schema = "baetoti")]
    public class Driver : BaseEntity
    {
        public long UserID { get; set; }
        public int DriverStatus { get; set; }
        public string Comments { get; set; }
        public string Nationality { get; set; }
        public string DOB { get; set; }
        public string IDNumber { get; set; }
        public string IDExpiryDate { get; set; }
        public string IDIssueDate { get; set; }
        public string FrontSideofIDPic { get; set; }
        public string BackSideofIDPic { get; set; }
        public string IBAN { get; set; }
        public string Title { get; set; }
        public string Gender { get; set; }
        public string PersonalPic { get; set; }
        public string DrivingLicensePic { get; set; }
        public string ExpirayDateofLicense { get; set; }
        public string VehicleRegistrationPic { get; set; }
        public string ExpiryDateofVehicleRegistration { get; set; }
        public string VehicleAuthorizationPic { get; set; }
        public string ExpiryDateofVehicleAuthorization { get; set; }
        public string MedicalCheckupPic { get; set; }
        public string ExpiryDateofMedicalcheckup { get; set; }
        public string VehicleInsurancePic { get; set; }
        public string SignaturePic { get; set; }
        public string FreelanceCertPic { get; set; }
        public string FreelanceCertID { get; set; }
        public DateTime FreelanceIssueDate { get; set; }
        public DateTime FreelanceExpireDate { get; set; }
        public string ExpiryDateofVehicleInsurance { get; set; }
        public string IdentityTypeId { get; set; }
        public string CarTypeId { get; set; }
        public string RegionId { get; set; }
        public string CityId { get; set; }
        public long CountryId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string MobileNumber { get; set; }
        public string CarNumber { get; set; }
        public string VehicleSequenceNumber { get; set; }
		public bool IsOnline { get; set; }
		public bool IsPublic { get; set; }
		public bool IsAcceptJob { get; set; }
		public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public bool MarkAsDeleted { get; set; }
        public int OnBoardingStatus { get; set; }
        public int? UpdatedByAdminId { get; set; }
    }
}
