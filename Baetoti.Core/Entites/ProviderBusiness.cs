using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("ProviderBusiness", Schema = "baetoti")]
    public class ProviderBusiness : BaseEntity
    {
        public long ProviderID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsCorpoarate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
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
        public int? UpdatedByAdminId { get; set; }
        public string BankAccountCertificate { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string BusinessCategory { get; set; }
        public DateTime CommercialStartDate { get; set; }
        public string TapCompanyStoreName { get; set; }
    }
}
