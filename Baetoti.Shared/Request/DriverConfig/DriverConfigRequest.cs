namespace Baetoti.Shared.Request.DriverConfig
{
    public class DriverConfigRequest
    {
        public long ID { get; set; }
		public decimal FromKM { get; set; }
		public decimal ToKM { get; set; }
		public decimal RatePerKM { get; set; }
		public decimal AdditionalRatePerKM { get; set; }
		public string Currency { get; set; }
		public decimal AdditionalKM { get; set; }
		public decimal MaximumDistance { get; set; }
        public decimal DriverComission { get; set; }
        public decimal ProviderComission { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal ServiceFeeFixed { get; set; }
    }
}
