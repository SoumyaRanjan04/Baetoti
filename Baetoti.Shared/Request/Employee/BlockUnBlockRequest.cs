namespace Baetoti.Shared.Request.Employee
{
    public class BlockUnBlockRequest
    {
        public long ID { get; set; }
        public bool IsBlocked { get; set; }
        public string Comments { get; set; }
    }
}
