using Baetoti.Shared.Response.Dashboard;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Analytic
{
    public class StatisticResponse
    {
        public RevenueVsCommision RevenueVsCommisions { get; set; }

        public TotalVsOnline TotalVsOnlines { get; set; }

        public List<LatestOrder> LatestOrders { get; set; }

        public List<DailyUser> DailyActiveUser { get; set; }

        public List<DailyUser> NewSignUp { get; set; }

        public List<DailyUser> UserActiveMinutes { get; set; }

        public OrderStats TotalOrder { get; set; } // Same will be used for Average Order Price

        public GeneralTopStats TopItem { get; set; }

        public GeneralTopStats TopCategory { get; set; }

        public GeneralTopStats TopSubCategory { get; set; }

        public GeneralTopStats TopBuyer { get; set; }

        public GeneralTopStats TopProvider { get; set; }

        public GeneralTopStats TopDriver { get; set; }

        public GeneralTopStats TopBuyerCount { get; set; }

        public GeneralTopStats TopProviderCount { get; set; }

        public GeneralTopStats TopDriverCount { get; set; }

        public StatisticResponse()
        {
            RevenueVsCommisions = new RevenueVsCommision();
            TotalVsOnlines = new TotalVsOnline();
            LatestOrders = new List<LatestOrder>();
            DailyActiveUser = new List<DailyUser>();
            NewSignUp = new List<DailyUser>();
            UserActiveMinutes = new List<DailyUser>();
            TotalOrder = new OrderStats();
            TopItem = new GeneralTopStats();
            TopCategory = new GeneralTopStats();
            TopSubCategory = new GeneralTopStats();
            TopBuyer = new GeneralTopStats();
            TopProvider = new GeneralTopStats();
            TopDriver = new GeneralTopStats();
        }
    }

    public class DailyUser
    {
        public string Date { get; set; }
        public int User { get; set; }
        public int Value { get; set; }
    }

    public class RevenueVsCommision
    {
        public decimal Cur { get; set; }
        public decimal Avg { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public List<RevenueVsCommisionData> Data { get; set; }
        public RevenueVsCommision()
        {
            Data = new List<RevenueVsCommisionData>();
        }
    }

    public class RevenueVsCommisionData
    {
        public string date { get; set; }

        public decimal Commision { get; set; }

        public int Revenue { get; set; }

    }

    public class OrderStats
    {
        public decimal Cur { get; set; }
        public decimal Avg { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public List<OrderData> Data { get; set; }
        public OrderStats()
        {
            Data = new List<OrderData>();
        }
    }

    public class OrderData
    {
        public string Date { get; set; }

        public decimal Price { get; set; }

        public decimal Average { get; set; }

    }

    public class GeneralTopStats
    {
        public decimal Cur { get; set; }
        public decimal Avg { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public List<string> Labels { get; set; }
        public List<decimal> Values { get; set; }
        public GeneralTopStats()
        {
            Labels = new List<string>();
            Values = new List<decimal>();
        }
    }

}
