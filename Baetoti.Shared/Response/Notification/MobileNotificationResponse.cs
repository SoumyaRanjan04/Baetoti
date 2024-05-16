using Baetoti.Shared.Enum;
using System;

namespace Baetoti.Shared.Response.Notification
{
    public class MobileNotificationResponse
    {
        public int Id { get; set; }
        public string ReceiverUser { get; set; }
        public UserType ReceiverUserType { get; set; }
        public string SenderUser { get; set; }
        public UserType SenderUserType { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }
        public string SubjectID { get; set; }
        public DateTime CreationTime { get; set; }
        public NotificationUser Receiver { get; set; }
        public NotificationUser Sender { get; set; }
        public MobileNotificationResponse()
        {
            Receiver = new NotificationUser();
            Sender = new NotificationUser();
        }
    }

    public class NotificationUser
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public string ProviderImage { get; set; }

        public string Image { get; set; }

        public bool IsOnline { get; set; }
    }

}
