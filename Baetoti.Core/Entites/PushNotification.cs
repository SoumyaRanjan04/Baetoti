using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("PushNotification", Schema = "baetoti")]
    public partial class PushNotification : BaseEntity
    {
        public long NotificationTypeID { get; set; }

        public string Title { get; set; }

        public string TitleArabic { get; set; }

        public string Text { get; set; }

        public string TextArabic { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public long UpdatedBy { get; set; }

        public DateTime LastUpdatedAt { get; set; }

    }
}
