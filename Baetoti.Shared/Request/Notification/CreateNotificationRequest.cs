namespace Baetoti.Shared.Request.Notification
{
    public class CreateNotificationRequest
    {
        public string DestUserID { get; set; }

        public long Type { get; set; }

        public int DestUserType { get; set; }

        public string NotificationTitle { get; set; }

        public string NotificationBody { get; set; }

        public string SenderUser { get; set; }

        public int SenderUserType { get; set; }

        public string SubjectID { get; set; }

    }
}
