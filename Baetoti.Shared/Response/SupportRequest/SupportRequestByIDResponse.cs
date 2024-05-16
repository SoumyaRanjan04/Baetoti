using System;

namespace Baetoti.Shared.Response.SupportRequest
{
    public class SupportRequestByIDResponse
    {
        public int SupportRequestType { get; set; }

        public string SupportRequestTypeValue { get; set; }

        public long ID { get; set; }

        public long UserID { get; set; }

        public string Title { get; set; }

        public string Picture { get; set; }

        public string Comments { get; set; }

        public string SupervisorComments { get; set; }

        public string SupervisorCommentsForUser { get; set; }

        public string ActionTaken { get; set; }

        public string RootCause { get; set; }

        public decimal UserRating { get; set; }

        public string UserFeedback { get; set; }

        public int SupportRequestStatus { get; set; }

        public string SupportUserName { get; set; }

        public string SupportUserDesignation { get; set; }

        public string SupportUserPicture { get; set; }

        public DateTime RecordDateTime { get; set; }

        public DateTime OpenDateTime { get; set; }

        public DateTime ResolveDateTime { get; set; }

        public long CSSID { get; set; }

        public string CSSName { get; set; }

    }
}
