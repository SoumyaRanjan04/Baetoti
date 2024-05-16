namespace Baetoti.Shared.Request.Notification
{
    public class GetAllNotificationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int UserType { get; set; } = 0;
    }
}
