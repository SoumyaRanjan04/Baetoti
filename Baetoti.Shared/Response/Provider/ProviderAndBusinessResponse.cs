using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.Provider
{
    public class ProviderAndBusinessResponse
    {
        public long UserID { get; set; }
        public long ProviderID { get; set; }
        public string MaroofID { get; set; }
        public string MaroofLink { get; set; }
        public string GovernmentID { get; set; }
        public string GovernmentIDPicture { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? ProviderCreatedAt { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ProfileImage { get; set; }
        public bool IsPublic { get; set; }
        public bool IsOnline { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string BussinessFirstName { get; set; }
        public string BusinessMiddleName { get; set; }
        public string BusinessLastName { get; set; }
        public string BusinessName { get; set; }

        public bool IsCorpoarate { get; set; }
        public DateTime ProviderBusinessCreatedAt { get; set; }
        public DateTime ProviderBusinessLastUpdatedAt { get; set; }
        public string Signature { get; set; }
        public bool IsCivilID { get; set; }
        public string CopyofIDOrPassport { get; set; }
        public string OwnerAdrress { get; set; }
        public string CommercialRegistrationLicense { get; set; }
        public string CompanyAddress { get; set; }
        public string VATRegistrationCertificate { get; set; }
        public string FreelanceCertificate { get; set; }
        public string IBANNumber { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BeneficiaryAddress { get; set; }
        public string SwiftCode { get; set; }
        public string CommercialNumber { get; set; }
        public DateTime CommercialExpiry { get; set; }
        public string VATRegistrationNumber { get; set; }
        public string BankAccountCertificate { get; set; }

        public long CountryID { get; set; }
        public string RegionID { get; set; }
        public string CityID { get; set; }

        public string CountryName { get; set; }
        public string RegionName { get; set; }
        public string CityName { get; set; }
        public string TapCompanyStoreName { get; set; }
    }
}
