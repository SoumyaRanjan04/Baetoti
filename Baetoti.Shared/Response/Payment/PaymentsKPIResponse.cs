using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.Payment
{
    public class TransactionChart
    {
        public string Date { get; set; }
        public double Revenue { get; set; }
        public double Commission { get; set; }
    }

    public class PaymentsKPIResponse
    {
        public int TotalAuthorizeRequests { get; set; }
        public int TotalChargeRequests { get; set; }
        public int TotalSuccessfulAuthorizeRequests { get; set; }
        public int TotalSuccessfulChargeRequests { get; set; }
        public int TotalFailedAuthorizeRequests { get; set; }
        public int TotalFailedChargeRequests { get; set; }
        public double TotalEarningsFromSuccessfulCharges { get; set; }
        public double TotalLossesFromFailedCharges { get; set; }
        public double TotalRevenue { get; set; }
        public double TotalRevenueLosses { get; set; }

        public double TotalRevenueFromProviders { get; set; }
        public double TotalRevenueFromCaptains { get; set; }
        public double TotalCommsFromProviders { get; set; }
        public double TotalCommsFromCaptains { get; set; }

        public List<TransactionChart> RevenueVSComms { get; set; }
    }
}
