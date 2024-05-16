using System.Collections.Generic;

namespace Baetoti.Shared.Response.Analytic
{
    public class FinanceResponse
    {
        public decimal TotalRevenu { get; set; }
        public decimal TotalCommision { get; set; }
        public decimal TotalTransaction { get; set; }
        public GeneralData AverageOrder { get; set; }
        public GeneralData CumulativeCommision { get; set; }
        public CommisionData Commision { get; set; }
        public GeneralData CumulativeSale { get; set; }
        public SaleData Sale { get; set; }
        public GeneralData CumulativeWallet { get; set; }
        public WalletData Wallet { get; set; }
        public FinanceResponse()
        {
            AverageOrder = new GeneralData();
            Commision = new CommisionData();
            CumulativeCommision = new GeneralData();
            Sale = new SaleData();
            CumulativeSale = new GeneralData();
            Wallet = new WalletData();
            CumulativeWallet = new GeneralData();
        }
    }

    public class GeneralData
    {
        public decimal Cur { get; set; }
        public decimal Avg { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public List<GeneralGraphData> GraphData { get; set; }
        public GeneralData()
        {
            GraphData = new List<GeneralGraphData>();
        }
    }

    public class GeneralGraphData
    {
        public decimal Value { get; set; }
        public string Date { get; set; }
        public decimal Avg { get; set; }
    }

    public class CommisionData
    {
        public GraphDetail Driver { get; set; }
        public GraphDetail Provider { get; set; }
        public CommisionData()
        {
            Driver = new GraphDetail();
            Provider = new GraphDetail();
        }
    }

    public class SaleData
    {
        public GraphDetail Order { get; set; }
        public GraphDetail Delivery { get; set; }
        public SaleData()
        {
            Order = new GraphDetail();
            Delivery = new GraphDetail();
        }
    }

    public class WalletData
    {
        public GraphDetail User { get; set; }
        public GraphDetail Baetoti { get; set; }
        public WalletData()
        {
            User = new GraphDetail();
            Baetoti = new GraphDetail();
        }
    }

    public class GraphDetail
    {
        public decimal Total { get; set; }
        public List<GeneralGraphData> GraphData { get; set; }
        public GraphDetail()
        {
            GraphData = new List<GeneralGraphData>();
        }
    }
}
