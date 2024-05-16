namespace Baetoti.Shared.Request.Chat
{
    public class SendMessageRequest
    {
        public string Message { get; set; }

        public string DestUser { get; set; }

        public int SenderUserType { get; set; }

        public int ReceiverUserType { get; set; }

        public bool HasAttachment { get; set; }

        public string AttachmentPath { get; set; }

    }
}
