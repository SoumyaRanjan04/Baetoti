using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("User", Schema = "baetoti")]
    public partial class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime? DOB { get; set; }
        public string Phone { get; set; }
        public string PhoneWithCountryCode { get; set; }
        public string CountryCode { get; set; }
        public string Address { get; set; }
        public long CountryID { get; set; }
        public string CityID { get; set; }
        public string RegionID { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string DefaultLocationTitle { get; set; }
        public int UserStatus { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? UpdatedByAdminId { get; set; }
        public bool IsProfileCompleted { get; set; }
        public bool IsSocialLoginUsed { get; set; }
        public bool IsOnline { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public string RefreshToken { get; set; }
        public int OnBoardingStatus { get; set; }
        public string UserDevice { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DefaultValue(1)]
        public int SelectedLanguage { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DefaultValue(1)]
        public int SelectedUserType { get; set; }

        public bool MarkAsDeleted { get; set; }
        public string MacAddress { get; set; }

    }
}
