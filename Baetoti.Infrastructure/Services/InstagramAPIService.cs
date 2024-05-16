using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Services;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Services.Base;
using Baetoti.Shared.Request.InstragramToken;
using Baetoti.Shared.Response.InstragramToken;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Services
{
    public class InstagramAPIService : BaseAPIService, IInstagramAPIService
    {

        private readonly BaetotiDbContext _dbContext;

        public InstagramAPIService(HttpClient httpClient, BaetotiDbContext dbContext) : base(httpClient)
        {
            _dbContext = dbContext;
        }

        public override async Task<TResponse> CallAPI<TRequest, TResponse>(string apiUrl, TRequest requestBody, HttpMethod httpMethod, HttpContext httpContext = null)
        {
            var responseBody = await InvokeAPI(apiUrl, requestBody, httpMethod, httpContext);
            return JsonConvert.DeserializeObject<TResponse>(responseBody);
        }

        public async Task<InstragramToken> Refresh(InstragramTokenRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            InstragramTokenResponse Response = await CallAPI<object, InstragramTokenResponse>($"{siteConfig.InstagramAPIURL}access_token?grant_type=ig_exchange_token&client_secret={request.InstagramAppSecret}&access_token={request.Token}", request, HttpMethod.Get, httpContext);
            return new InstragramToken
            {
                StoreID = request.StoreID,
                Token = Response.access_token
            };
        }

    }
}
