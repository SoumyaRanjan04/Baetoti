using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Services.Base;
using Baetoti.Shared.Request.Payment;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Services
{
    public interface IPaymentAPIService : IBaseAPIService
    {
        Task<SharedResponse> ChargeCard(ChargeCardRequest request, HttpContext httpContext, SiteConfig siteConfig);
        Task<SharedResponse> GetAllAuthorizedPayments(GetAllAuthPaymentsRequest request, HttpContext httpContext, SiteConfig siteConfig);
        Task<SharedResponse> GetAllSucessfulTransactions(GetAllCapturePaymentsRequest request, HttpContext httpContext, SiteConfig siteConfig);

        Task<SharedResponse> GetAuthPaymentByID(long ID, HttpContext httpContext, SiteConfig siteConfig);
        Task<SharedResponse> GetCapturedPaymentByID(long ID, HttpContext httpContext, SiteConfig siteConfig);
        Task<SharedResponse> GetPaymentKPIS(DateTime StartDate, DateTime EndDate, HttpContext httpContext, SiteConfig siteConfig);

    }
}
