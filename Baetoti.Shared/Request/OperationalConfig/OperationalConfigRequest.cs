namespace Baetoti.Shared.Request.OperationalConfig
{
    public class OperationalConfigRequest
    {
        public long ID { get; set; }

        public string Title { get; set; }

        public string Notes { get; set; }

        public long CountryID { get; set; }

        public string Region { get; set; }

        public string RegionID { get; set; }

        public string City { get; set; }

        public string CityID { get; set; }

        public int FenceStatus { get; set; }

    }

}
