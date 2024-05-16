namespace Baetoti.Shared.Request.Email
{
    public class EmailTemplateRequest
    {
        public long ID { get; set; }

        public string Subject { get; set; }

        public string HtmlBody { get; set; }

    }
}
