using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("Promotion", Schema = "baetoti")]
    public partial class Promotion: BaseEntity
    {
        public int PromotionType { get; set; }

        public string PromoCodeName { get; set; }
        
        public string PromoCodeNameArabic { get; set; }

        public string PromoCodeDescription { get; set; }
        
        public string PromoCodeDescriptionArabic { get; set; }

        public int DiscountType { get; set; }

        public decimal DiscountValue { get; set; }

        public int MinimumBasketValue { get; set; }

        public string PromoBearer { get; set; }

        public decimal Commision { get; set; }

        public int CommisionType { get; set; }

        public int NumberOfVoucher { get; set; }

        public int NumberOfRedeems { get; set; }

        public DateTime PromoCodeValidity { get; set; }

        public int PromotionStatus { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

    }
}
