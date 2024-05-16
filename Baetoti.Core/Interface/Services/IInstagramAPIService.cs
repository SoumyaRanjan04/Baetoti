using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Services.Base;
using Baetoti.Shared.Request.InstragramToken;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Services
{
    public interface IInstagramAPIService : IBaseAPIService
    {
        Task<InstragramToken> Refresh(InstragramTokenRequest request, HttpContext httpContext, SiteConfig siteConfig);

    }

}
