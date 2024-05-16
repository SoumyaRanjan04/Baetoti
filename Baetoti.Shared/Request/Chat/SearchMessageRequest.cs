using System.Collections.Generic;

namespace Baetoti.Shared.Request.Chat
{
    public class SearchMessageRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public List<string> UserIDs { get; set; }
        public string SearchValue { get; set; }
    }
}
