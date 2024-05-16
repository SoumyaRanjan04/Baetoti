using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Services;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Services.Base;
using Baetoti.Shared.Request.Payment;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Drawing.Printing;
using System.Net.Http;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Services
{
    public class PaymentAPIService : BaseAPIService, IPaymentAPIService
    {

        private readonly BaetotiDbContext _dbContext;

        public PaymentAPIService(HttpClient httpClient, BaetotiDbContext dbContext) : base(httpClient)
        {
            _dbContext = dbContext;
        }

        public override async Task<TResponse> CallAPI<TRequest, TResponse>(string apiUrl, TRequest requestBody, HttpMethod httpMethod, HttpContext httpContext = null)
        {
            var responseBody = await InvokeAPI(apiUrl, requestBody, httpMethod, httpContext);
            return JsonConvert.DeserializeObject<TResponse>(responseBody);
        }

        public async Task<SharedResponse> ChargeCard(ChargeCardRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.PaymentAPIURL}Payment/ChargeCard", request, HttpMethod.Post, httpContext);
        }
        public async Task<SharedResponse> GetAllAuthorizedPayments(GetAllAuthPaymentsRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            string url = $"{siteConfig.PaymentAPIURL}Payment/GetAllAuthorizedPaymentsForAdmin?PageSize={request.PageSize}&PageNumber={request.PageNumber}&StartDate={request.StartDate}&EndDate={request.EndDate}";
            return await CallAPI<object, SharedResponse>(url, null, HttpMethod.Get, httpContext);
        }

        public async Task<SharedResponse> GetAllSucessfulTransactions(GetAllCapturePaymentsRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            string url = $"{siteConfig.PaymentAPIURL}Payment/GetAllCapturedTransactionsForAdmin?PageSize={request.PageSize}&PageNumber={request.PageNumber}&StartDate={request.StartDate}&EndDate={request.EndDate}";
            return await CallAPI<object, SharedResponse>(url, null, HttpMethod.Get, httpContext);
        }

        public async Task<SharedResponse> GetAuthPaymentByID(long ID, HttpContext httpContext, SiteConfig siteConfig)
        {
            string url = $"{siteConfig.PaymentAPIURL}Payment/GetPaymentAuthRequestByID?ID={ID}";
            return await CallAPI<object, SharedResponse>(url, null, HttpMethod.Get, httpContext);
        }
        public async Task<SharedResponse> GetCapturedPaymentByID(long ID, HttpContext httpContext, SiteConfig siteConfig)
        {
            string url = $"{siteConfig.PaymentAPIURL}Payment/GetPaymentCaptureByID?ID={ID}";
            return await CallAPI<object, SharedResponse>(url, null, HttpMethod.Get, httpContext);
        }

        public async Task<SharedResponse> GetPaymentKPIS(DateTime StartDate, DateTime EndDate, HttpContext httpContext, SiteConfig siteConfig)
        {
            string url = $"{siteConfig.PaymentAPIURL}Payment/GetKPIS?StartDate={StartDate}&EndDate={EndDate}";
            return await CallAPI<object, SharedResponse>(url, null, HttpMethod.Get, httpContext);
        }

    }
}
