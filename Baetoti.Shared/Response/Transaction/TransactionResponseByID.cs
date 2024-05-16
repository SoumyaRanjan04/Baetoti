using System;

namespace Baetoti.Shared.Response.Transaction
{
    public class TransactionResponseByID
    {
        public long TransactionID { get; set; }
        public long UserID { get; set; }
        public long OrderID { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionFrom { get; set; }
        public string TransactionTo { get; set; }
        public string TransactionFor { get; set; }
        public string PaymentType { get; set; }
        public DateTime TransactionTime { get; set; }
        public string ProviderInvoice { get; set; }
        public string DriverInvoice { get; set; }
        public string UserInvoice { get; set; }
        public int OrderStatus { get; set; }
        public decimal DriverCommision { get; set; }
        public decimal ProviderCommision { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal Discount { get; set; }
        public decimal DeliveryCharges { get; set; }
    }
}
