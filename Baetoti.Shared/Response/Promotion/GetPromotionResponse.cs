using Baetoti.Shared.Response.Dashboard;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Promotion
{
	public class GetPromotionResponse
	{
        public int Total { get; set; }

        public int Ongoing { get; set; }

        public int Expired { get; set; }

        public List<Promo> ExistingPromo { get; set; }

		public List<Promo> ExpiredPromo { get; set; }

		public TotalVsOnline TotalVsOnlines { get; set; }

		public List<Sale> Sales { get; set; }

		public List<RevenueVsCommision> RevenueVsCommisions { get; set; }

		public GetPromotionResponse()
		{
			ExistingPromo = new List<Promo>();
			ExpiredPromo = new List<Promo>();
			TotalVsOnlines = new TotalVsOnline();
			Sales = new List<Sale>();
			RevenueVsCommisions = new List<RevenueVsCommision>();
		}
	}

	public class Promo
	{
		public long ID { get; set; }

		public string User { get; set; }

		public string PromoStart { get; set; }

		public string PromoExpiration { get; set; }

		public int TotalSignups { get; set; }

		public string By { get; set; }

		public int PromoStatus { get; set; }

	}
}
