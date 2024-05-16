using System.Collections.Generic;
using Baetoti.Shared.Enum;

namespace Baetoti.Shared.Request.Notification
{
    public class ServiceNotificationRequest
    {
        public string NotificationTitle { get; set; }

        public string NotificationBody { get; set; }

        public List<string> UserIDs { get; set; }
        public List<UserType> UserTypes { get; set; }
        
        public ServiceNotificationRequest()
        {
            UserIDs = new List<string>();
            UserTypes = new List<UserType>();
        }
    }
}
