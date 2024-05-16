using Baetoti.Shared.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Baetoti.Shared.Response.Payment
{
    public class SucessfulTransactionResponse
    {
        public long ID { get; set; }
        public string UserID { get; set; }
        public float amount { get; set; }
        public string currency { get; set; }
        public DateTime dateTime { get; set; }
        public string orderID { get; set; }
        public UserType userType { get; set; }
        public string Status { get; set; }
        public string CardType { get; set; }
        public string StatusDescription { get; set; }
        public string AuthID { get; set; }
        public AuthEntryResponse AuthEntry { get; set; }

        public string FromName { get; set; }
        public string ToName { get; set; }

        public string FromID { get; set; }
        public string ToID { get; set; }

        public string OrderStatus { get; set; }
    }
}
