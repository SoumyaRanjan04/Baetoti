using System.Net.Mail;

namespace Baetoti.Shared.Request.Email
{
    public class EmailRequest
    {
        public string Receiver { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string UserID { get; set; }

        public string API { get; set; }

        public MailPriority MailPriority { get; set; }

        public EmailRequest()
        {
            MailPriority = MailPriority.Normal;
        }

    }
}
