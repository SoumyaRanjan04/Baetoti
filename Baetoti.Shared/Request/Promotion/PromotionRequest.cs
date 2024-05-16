using System;

namespace Baetoti.Shared.Request.Promotion
{
    public class PromotionRequest
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
    }
}
