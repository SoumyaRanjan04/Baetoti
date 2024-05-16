namespace Baetoti.Shared.Request.Payment
{
    public class ChargeCardRequest
    {
        public string BaetotiOrderID { get; set; }

        public string BaetotiProviderID { get; set; }

        public string BaetotiDriverID { get; set; }

    }
}
