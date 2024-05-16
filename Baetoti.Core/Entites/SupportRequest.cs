using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("SupportRequest", Schema = "baetoti")]
    public partial class SupportRequest : BaseEntity
    {
        public int SupportRequestType { get; set; }

        public long UserID { get; set; }

        public string Title { get; set; }

        public string Picture { get; set; }

        public string Comments { get; set; }

        public string SupervisorComments { get; set; }

        public string SupervisorCommentsForUser { get; set; }

        public long SupervisorID { get; set; }

        public string ActionTaken { get; set; }

        public string RootCause { get; set; }

        public decimal UserRating { get; set; }

        public string UserFeedback { get; set; }

        public int SupportRequestStatus { get; set; }

        public DateTime RecordDateTime { get; set; }

        public DateTime OpenDateTime { get; set; }

        public DateTime ResolveDateTime { get; set; }

    }
}
