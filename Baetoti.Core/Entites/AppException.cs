using Baetoti.Core.Entites.Base;
using Baetoti.Shared.Extentions;
using System;

namespace Baetoti.Core.Entites
{
    public class AppException : BaseEntity
    {
        public string UserId { get; set; } //null if exception occur before authentication

        public string Url { get; set; }

        public string Type { get; set; }

        public string Message { get; set; }

        public string InnerException { get; set; }

        public string StackTrace { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now.ToTimeZoneTime("Arab Standard Time");

        public int UserType { get; set; }

        public string RequestType { get; set; }

        public string RequestBody { get; set; }

    }
}
