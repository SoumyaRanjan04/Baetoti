using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Services.Base
{
    public interface IBaseAPIService
    {
        Task<string> InvokeAPI<TRequest>(string apiUrl, TRequest requestBody, HttpMethod httpMethod, HttpContext httpContext = null);

        Task<TResponse> CallAPI<TRequest, TResponse>(string apiUrl, TRequest requestBody, HttpMethod httpMethod, HttpContext httpContext = null);

    }
}
