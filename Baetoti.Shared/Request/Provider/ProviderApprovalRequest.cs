namespace Baetoti.Shared.Request.Provider
{
    public class ProviderApprovalRequest
    {
        public long UserID { get; set; }
        public int StatusValue { get; set; }
        public string Comments { get; set; }
    }
}
