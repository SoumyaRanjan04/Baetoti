using Baetoti.Shared.Response.Dashboard;
using Baetoti.Shared.Response.User;
using System;
using System.Collections.Generic;
using static Baetoti.Shared.Response.Promotion.ViewPromotionResponse;

namespace Baetoti.Shared.Response.Promotion
{
	public class ViewPromotionResponse
	{
        public PromoDetail PromoDetails { get; set; }

        public List<UserList> userList { get; set; }

		public List<BuyerHistory> buyerHistory { get; set; }

		public TotalVsOnline TotalVsOnlines { get; set; }

		public List<Sale> Sales { get; set; }

		public List<RevenueVsCommision> RevenueVsCommisions { get; set; }

		public ViewPromotionResponse()
		{
			PromoDetails = new PromoDetail();
			userList = new List<UserList>();
			buyerHistory = new List<BuyerHistory>();
			TotalVsOnlines = new TotalVsOnline();
			Sales = new List<Sale>();
			RevenueVsCommisions = new List<RevenueVsCommision>();
		}

		public class PromoDetail
		{
			public string PromotionType { get; set; }

			public string PromoCodeName { get; set; }

			public string PromoCodeNameArabic { get; set; }

			public string PromoCodeDescription { get; set; }

			public string PromoCodeDescriptionArabic { get; set; }

			public string DiscountType { get; set; }

			public decimal DiscountValue { get; set; }

			public int MinimumBasketValue { get; set; }

			public string PromoBearer { get; set; }

			public decimal Commision { get; set; }

			public string CommisionType { get; set; }

			public int NumberOfVoucher { get; set; }

			public int NumberOfRedeems { get; set; }

			public DateTime PromoCodeValidity { get; set; }

		}

	}
}
