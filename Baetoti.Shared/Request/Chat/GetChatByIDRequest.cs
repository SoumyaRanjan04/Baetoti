namespace Baetoti.Shared.Request.Chat
{
    public class GetChatByIDRequest
    {
        public long ID { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int SenderUserType { get; set; }

        public int ReceiverUserType { get; set; }

    }
}
