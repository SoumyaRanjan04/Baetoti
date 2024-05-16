using Baetoti.Shared.Enum;
using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Chat
{
    public class ChatThreadResponse
    {
        public bool IsThreadBlocked { get; set; }
        public bool IsThreadMuted { get; set; }
        public string BlockedByUser { get; set; }
        public string MutedByUser { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }
        public string SenderUser { get; set; }
        public string ReceiverUser { get; set; }
        public UserType SenderUserType { get; set; }
        public UserType RecevierUserType { get; set; }
        public bool isRead { get; set; }
        public DateTime LastMessageTime { get; set; }
    }

    public class ChatMessage
    {
        public long ChatThreadID { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public ChatThreadResponse parentThread { get; set; }
        public bool IsFileAttachment { get; set; }
        public string FileAttachmentPath { get; set; }
    }

}
