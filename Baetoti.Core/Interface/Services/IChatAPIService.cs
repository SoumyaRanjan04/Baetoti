using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Services.Base;
using Baetoti.Shared.Request.Chat;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Services
{
    public interface IChatAPIService : IBaseAPIService
    {
        Task<SharedResponse> SendMessage(SendMessageRequest request, HttpContext httpContext, SiteConfig siteConfig);

        Task<SharedResponse> Mute(UserIDRequest request, HttpContext httpContext, SiteConfig siteConfig);

        Task<SharedResponse> UnMute(UserIDRequest request, HttpContext httpContext, SiteConfig siteConfig);

        Task<SharedResponse> UnBlock(UserIDRequest request, HttpContext httpContext, SiteConfig siteConfig);

        Task<SharedResponse> Block(UserIDRequest request, HttpContext httpContext, SiteConfig siteConfig);

        Task<PaginationResponse> GetAllChat(PaginationRequest request, HttpContext httpContext, SiteConfig siteConfig);

        Task<PaginationResponse> SearchMessageRequest(SearchMessageRequest request, HttpContext httpContext, SiteConfig siteConfig);

        Task<SharedResponse> GetByID(GetChatByIDRequest request, HttpContext httpContext, SiteConfig siteConfig);

    }
}
