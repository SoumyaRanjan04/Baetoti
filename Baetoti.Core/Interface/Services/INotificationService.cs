using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Services.Base;
using Baetoti.Shared.Request.Notification;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Services
{
    public interface INotificationService : IBaseAPIService
    {
        Task<SharedResponse> GetNotificationToken(long UserID, HttpContext httpContext, SiteConfig siteConfig);

        Task<PaginationResponse> GetNotifications(GetAllNotificationRequest request, HttpContext httpContext, SiteConfig siteConfig);

        Task<SharedResponse> UpdateNotificationToken(UpdateNotificationTokenRequest request, HttpContext httpContext, SiteConfig siteConfig);

        Task<SharedResponse> MarkNotificationAsRead(RequestID request, HttpContext httpContext, SiteConfig siteConfig);

        Task<SharedResponse> CreatePushNotification(CreateNotificationRequest request, HttpContext httpContext, SiteConfig siteConfig);
        
        Task<SharedResponse> CreateServiceNotification(ServiceNotificationRequest request, HttpContext httpContext, SiteConfig siteConfig);

        Task<SharedResponse> RelayUserSignout(string userID, HttpContext httpContext, SiteConfig siteConfig);

    }
}
