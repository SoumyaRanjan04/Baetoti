using Baetoti.Shared.Enum;
using System;

namespace Baetoti.Shared.Response.Chat
{
    public class ChatResponse
    {
        public ChatUser Sender { get; set; }

        public ChatUser Receiver { get; set; }

        public UserType SenderUserType { get; set; }

        public UserType RecevierUserType { get; set; }

        public ChatMessage LastMessage { get; set; }

        public bool isRead { get; set; }

        public bool IsThreadBlocked { get; set; }

        public bool IsThreadMuted { get; set; }

        public string BlockedByUser { get; set; }

        public DateTime LastMessageTime { get; set; }

        public string MutedByUser { get; set; }

        public ChatResponse()
        {
            Sender = new ChatUser();
            Receiver = new ChatUser();
            LastMessage = new ChatMessage();
        }
    }

    public class ChatUser
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public string ProviderImage { get; set; }

        public string Image { get; set; }

        public bool IsOnline { get; set; }
    }

}
