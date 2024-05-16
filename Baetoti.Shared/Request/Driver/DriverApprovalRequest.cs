namespace Baetoti.Shared.Request.Driver
{
    public class DriverApprovalRequest
    {
        public long UserID { get; set; }
        public int StatusValue { get; set; }
        public string Comments { get; set; }
    }
}
