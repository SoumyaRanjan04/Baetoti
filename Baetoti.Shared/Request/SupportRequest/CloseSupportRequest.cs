namespace Baetoti.Shared.Request.SupportRequest
{
    public class CloseSupportRequest
    {
        public long ID { get; set; }

        public string SupervisorComments { get; set; }

        public string SupervisorCommentsForUser { get; set; }

        public string ActionTaken { get; set; }

        public string RootCause { get; set; }

    }
}
