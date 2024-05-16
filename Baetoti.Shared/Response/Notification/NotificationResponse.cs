using System;

namespace Baetoti.Shared.Response.Notification
{
    public class NotificationResponse
    {
        public long ID { get; set; }

        public long UserID { get; set; }

        public string Name { get; set; }
        public string UserName { get; set; }

        public string NameArabic { get; set; }

        public string Title { get; set; }

        public string TitleArabic { get; set; }

        public string Text { get; set; }

        public string TextArabic { get; set; }

        public string SentBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserPicture { get; set; }

    }
}
