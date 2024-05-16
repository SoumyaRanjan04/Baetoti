using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("Notification", Schema = "baetoti")]
    public partial class Notification : BaseEntity
    {
        public string Name { get; set; }

        public string NameArabic { get; set; }

        public string Title { get; set; }

        public string TitleArabic { get; set; }

        public string Text { get; set; }

        public string TextArabic { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public long UserID { get; set; }

        public int SentCount { get; set; }

        public int UserType { get; set; }

    }
}
