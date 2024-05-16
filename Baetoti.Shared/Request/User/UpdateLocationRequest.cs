namespace Baetoti.Shared.Request.User
{
    public class UpdateLocationRequest
    {
        public long ID { get; set; }

        public string Address { get; set; }

        public string Title { get; set; }

        public bool IsSelected { get; set; }

        public Coordinates coordinates { get; set; }
    }

    public class Coordinates
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
