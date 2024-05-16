using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Services.Base;
using Baetoti.Shared.Response.GovtAPI;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Services
{
    public interface IGovtAPIService : IBaseAPIService
    {
        Task<SharedLookupResponse[]> GetAuthorities(SiteConfig siteConfig);

        Task<SharedLookupResponse[]> GetCancellationReasons(SiteConfig siteConfig);

        Task<SharedLookupResponse[]> GetRegions(SiteConfig siteConfig);

        Task<SharedLookupResponse[]> GetCategories(SiteConfig siteConfig);

        Task<SharedLookupResponse[]> GetIdentityTypes(SiteConfig siteConfig);

        Task<SharedLookupResponse[]> GetPaymentMethods(SiteConfig siteConfig);

        Task<SharedLookupResponse[]> GetCarTypes(SiteConfig siteConfig);

        Task<SharedLookupResponse[]> GetCountries(SiteConfig siteConfig);

        Task<SharedLookupResponse[]> GetCities(SiteConfig siteConfig, string regionID);

    }
}
