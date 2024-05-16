using System;

namespace Baetoti.Shared.Request.User
{
    public class OnBoardingRequest
    {
        public string Key { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

    }
}
