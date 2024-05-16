using System.Collections.Generic;
using Baetoti.Shared.Enum;

namespace Baetoti.Shared.Request.Notification
{
    public class NotificationRequest
    {
        public string Name { get; set; }

        public string NameArabic { get; set; }

        public string Title { get; set; }

        public string TitleArabic { get; set; }

        public string Text { get; set; }

        public string TextArabic { get; set; }

        public int UserType { get; set; }

      //  public long FenceID { get; set; }

        public long CountryID { get; set; }

        public string CityID { get; set; }
        public string RegionID { get; set; }

        public string Gender { get; set; }

       // public int OrderStatus { get; set; }

        public List<string> UserIDs { get; set; }
        public List<UserType> UserTypeList { get; set; }

        public NotificationRequest()
        {
            UserIDs = new List<string>();
            UserTypeList = new List<UserType>();
        }

    }
}
