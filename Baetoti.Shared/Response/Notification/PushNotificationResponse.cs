namespace Baetoti.Shared.Response.Notification
{
    public class PushNotificationResponse
    {
        public long ID { get; set; }

        public long NotificationTypeID { get; set; }

        public string NotificationType { get; set; }

        public string Title { get; set; }

        public string TitleArabic { get; set; }

        public string Text { get; set; }

        public string TextArabic { get; set; }

    }
}
