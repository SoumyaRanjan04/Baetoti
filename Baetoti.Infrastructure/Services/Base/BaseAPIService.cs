using Baetoti.Core.Interface.Services.Base;
using Baetoti.Shared.Response.GovtAPI;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Services.Base
{
    public abstract class BaseAPIService : IBaseAPIService
    {

        private readonly HttpClient _httpClient;

        public BaseAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> InvokeAPI<TRequest>(string apiUrl, TRequest requestBody, HttpMethod httpMethod, HttpContext httpContext = null)
        {
            var request = new HttpRequestMessage(httpMethod, apiUrl);

            if (requestBody != null)
            {
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                request.Content = content;
            }

            if (httpContext != null)
            {
                string bearerToken = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public abstract Task<TResponse> CallAPI<TRequest, TResponse>(string apiUrl, TRequest requestBody, HttpMethod httpMethod, HttpContext httpContext = null);

    }
}
