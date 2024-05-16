using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Services;
using Baetoti.Infrastructure.Services.Base;
using Baetoti.Shared.Request.GovtAPI;
using Baetoti.Shared.Response.GovtAPI;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Baetoti.Infrastructure.Services
{
    public class GovtAPIService : BaseAPIService, IGovtAPIService
    {

        public GovtAPIService(HttpClient httpClient) : base(httpClient) { }

        public override async Task<TResponse> CallAPI<TRequest, TResponse>(string apiUrl, TRequest requestBody, HttpMethod httpMethod, HttpContext httpContext = null)
        {
            var responseBody = await InvokeAPI(apiUrl, requestBody, httpMethod);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<TResponse>>(responseBody);
            return apiResponse.Data;
        }

        public async Task<SharedLookupResponse[]> GetAuthorities(SiteConfig siteConfig)
        {
            SharedLookupRequest request = new SharedLookupRequest
            {
                password = siteConfig.GovtAPIPassword,
                companyName = siteConfig.GovtAPICompanyName
            };
            return await CallAPI<object, SharedLookupResponse[]>($"{siteConfig.GovtAPIURL}Lookup/authorities-list", request, HttpMethod.Post);
        }

        public async Task<SharedLookupResponse[]> GetCancellationReasons(SiteConfig siteConfig)
        {
            SharedLookupRequest request = new SharedLookupRequest
            {
                password = siteConfig.GovtAPIPassword,
                companyName = siteConfig.GovtAPICompanyName
            };
            return await CallAPI<object, SharedLookupResponse[]>($"{siteConfig.GovtAPIURL}Lookup/cancellation-reasons-list", request, HttpMethod.Post);
        }

        public async Task<SharedLookupResponse[]> GetRegions(SiteConfig siteConfig)
        {
            SharedLookupRequest request = new SharedLookupRequest
            {
                password = siteConfig.GovtAPIPassword,
                companyName = siteConfig.GovtAPICompanyName
            };
            return await CallAPI<object, SharedLookupResponse[]>($"{siteConfig.GovtAPIURL}Lookup/regions-list", request, HttpMethod.Post);
        }

        public async Task<SharedLookupResponse[]> GetCategories(SiteConfig siteConfig)
        {
            SharedLookupRequest request = new SharedLookupRequest
            {
                password = siteConfig.GovtAPIPassword,
                companyName = siteConfig.GovtAPICompanyName
            };
            return await CallAPI<object, SharedLookupResponse[]>($"{siteConfig.GovtAPIURL}Lookup/categories-list", request, HttpMethod.Post);
        }

        public async Task<SharedLookupResponse[]> GetIdentityTypes(SiteConfig siteConfig)
        {
            SharedLookupRequest request = new SharedLookupRequest
            {
                password = siteConfig.GovtAPIPassword,
                companyName = siteConfig.GovtAPICompanyName
            };
            return await CallAPI<object, SharedLookupResponse[]>($"{siteConfig.GovtAPIURL}Lookup/identity-types-list", request, HttpMethod.Post);
        }

        public async Task<SharedLookupResponse[]> GetPaymentMethods(SiteConfig siteConfig)
        {
            SharedLookupRequest request = new SharedLookupRequest
            {
                password = siteConfig.GovtAPIPassword,
                companyName = siteConfig.GovtAPICompanyName
            };
            return await CallAPI<object, SharedLookupResponse[]>($"{siteConfig.GovtAPIURL}Lookup/payment-methods-list", request, HttpMethod.Post);
        }

        public async Task<SharedLookupResponse[]> GetCarTypes(SiteConfig siteConfig)
        {
            SharedLookupRequest request = new SharedLookupRequest
            {
                password = siteConfig.GovtAPIPassword,
                companyName = siteConfig.GovtAPICompanyName
            };
            return await CallAPI<object, SharedLookupResponse[]>($"{siteConfig.GovtAPIURL}Lookup/car-types-list", request, HttpMethod.Post);
        }

        public async Task<SharedLookupResponse[]> GetCountries(SiteConfig siteConfig)
        {
            SharedLookupRequest request = new SharedLookupRequest
            {
                password = siteConfig.GovtAPIPassword,
                companyName = siteConfig.GovtAPICompanyName
            };
            return await CallAPI<object, SharedLookupResponse[]>($"{siteConfig.GovtAPIURL}Lookup/countries-list", request, HttpMethod.Post);
        }

        public async Task<SharedLookupResponse[]> GetCities(SiteConfig siteConfig, string regionID)
        {
            CityRequest request = new CityRequest
            {
                credential = new Credential
                {
                    password = siteConfig.GovtAPIPassword,
                    companyName = siteConfig.GovtAPICompanyName
                },
                regionId = regionID
            };
            return await CallAPI<object, SharedLookupResponse[]>($"{siteConfig.GovtAPIURL}Lookup/cities-list", request, HttpMethod.Post);
        }

    }

}
