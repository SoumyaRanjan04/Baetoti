using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.Payment
{
    public class AuthEntryResponse
    {
        public long ID { get; set; }
        public string UserID { get; set; }
        public string AuthID { get; set; }
        public string BaetotiOrderID { get; set; }

        public float Amount { get; set; }

        public float ProviderAmount { get; set; }
        public float DriverAmount { get; set; }
        public string Description { get; set; }
        public string StatementDescription { get; set; }

        public string TapCustomerID { get; set; }

        public string Status { get; set; }

        public string CardType { get; set; }
        public DateTime DateTime { get; set; }

        public string BuyerName { get; set; }
        public string ProviderName { get; set; }
        public string ProviderID { get; set; }
        public string BuyerID { get; set; }
        public bool DidItCreateOrder { get; set; }
        public decimal ServiceFees { get; set; }
    }
}
