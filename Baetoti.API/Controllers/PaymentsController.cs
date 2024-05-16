using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Core.Interface.Services;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Services;
using Baetoti.Shared.Request.Payment;
using Baetoti.Shared.Request.User;
using Baetoti.Shared.Response.Chat;
using Baetoti.Shared.Response.Payment;
using Baetoti.Shared.Response.Shared;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
    public class PaymentsController : ApiBaseController
    {
        public readonly IPaymentAPIService _paymentAPIService;
        public readonly IOrderItemRepository _orderItemRepo;
        public readonly IProviderRepository _providerRepo;
        public readonly IDriverRepository _driverRepo;
        public PaymentsController(
            IPaymentAPIService paymentService,
            IOrderItemRepository orderRepo,
            IProviderRepository providerRepo,
            IDriverRepository driverRepository,
            ISiteConfigService siteConfigService
            ) : base(siteConfigService)
        {
            _paymentAPIService = paymentService;
            _orderItemRepo = orderRepo;
            _providerRepo = providerRepo;
            _driverRepo = driverRepository;
        }

        [HttpGet("GetPaymentKPIS")]
        public async Task<IActionResult> GetPaymentKPIS(DateTime StartDate, DateTime EndDate)
        {
            var resp = await _paymentAPIService.GetPaymentKPIS(StartDate, EndDate, HttpContext, _siteConfig);
            var KPIS = JsonConvert.DeserializeObject<PaymentsKPIResponse>(Convert.ToString(resp.Record));

            return Ok(new SharedResponse(true, 200, "success", KPIS));
        }

        [HttpGet("GetAuthorizedPaymentByID")]
        public async Task<IActionResult> GetAuthorizedPaymentByID(long ID)
        {
            var resp = await _paymentAPIService.GetAuthPaymentByID(ID, HttpContext, _siteConfig);
            var authEntry = JsonConvert.DeserializeObject<AuthEntryResponse>(Convert.ToString(resp.Record));

            if (authEntry.BaetotiOrderID != null && authEntry.BaetotiOrderID != "")
            {
                authEntry.DidItCreateOrder = true;
                var order = await _orderItemRepo.GetUserOrderByID(int.Parse(authEntry.BaetotiOrderID));

                authEntry.BuyerName = order.buyer.Name;
                authEntry.ProviderName = order.provider.Name;
                authEntry.ServiceFees = order.ServiceFees;
            }
            else
            {
                authEntry.DidItCreateOrder = false;
            }

            return Ok(new SharedResponse(true, 200, "success", authEntry));
        }
        [HttpGet("GetCapturedPaymentByID")]
        public async Task<IActionResult> GetCapturedPaymentByID(long ID)
        {
            var resp = await _paymentAPIService.GetCapturedPaymentByID(ID, HttpContext, _siteConfig);
            var transaction = JsonConvert.DeserializeObject<SucessfulTransactionResponse>(Convert.ToString(resp.Record));

            var order = await _orderItemRepo.GetByID(int.Parse(transaction.orderID));

            transaction.FromName = order.customerDetail.Name;

            if (transaction.UserID == "BAETOTI")
            {
                transaction.ToName = "Baetoti (commission)";
            }
            else if (transaction.userType == Shared.Enum.UserType.Provider)
            {
                transaction.ToName = order.providerDetail.Name;
            }
            else if (transaction.userType == Shared.Enum.UserType.Driver)
            {
                transaction.ToName = order.driverDetail.Name;
            }

            transaction.AuthEntry.BuyerName = order.customerDetail.Name;
            transaction.AuthEntry.ProviderName = order.providerDetail.Name;
            transaction.AuthEntry.DidItCreateOrder = true;
            transaction.OrderStatus = order.orderStatus.OrderStatus;
            return Ok(new SharedResponse(true, 200, "success", transaction));
        }

        [HttpPost("GetAllAuthorizedPayments")]
        public async Task<IActionResult> GetAllAuthorizedPayments([FromBody]GetAllAuthPaymentsRequest request)
        {
            try
            {
                var resp = await _paymentAPIService.GetAllAuthorizedPayments(request, HttpContext, _siteConfig);
                PaginationResponse pageResp = JsonConvert.DeserializeObject<PaginationResponse>(Convert.ToString(resp.Record));

                List <AuthEntryResponse> data = JsonConvert.DeserializeObject<List<AuthEntryResponse>>(Convert.ToString(pageResp.Data));
                foreach (var authEntry in data)
                {
                    if (authEntry.BaetotiOrderID != null && authEntry.BaetotiOrderID != "")
                    {
                        authEntry.DidItCreateOrder = true;
                        var order = await _orderItemRepo.GetUserOrderByID(int.Parse(authEntry.BaetotiOrderID));

                        authEntry.BuyerName = order.buyer.Name;
                        authEntry.ProviderName = order.provider.Name;
                        authEntry.ServiceFees = order.ServiceFees;
                        authEntry.ProviderID = order.provider.UserID.ToString();
                        authEntry.BuyerID = order.buyer.UserID.ToString();
                    }
                    else
                    {
                        authEntry.DidItCreateOrder = false;
                    }
                }
                pageResp.Data = data;
                return Ok(new SharedResponse(true, 200, "success", pageResp));
            }
            catch (System.Exception ex)
            {

                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("GetAllCapturedPayments")]
        public async Task<IActionResult> GetAllCapturedPayments([FromBody]GetAllCapturePaymentsRequest request)
        {
            try
            {
                var resp = await _paymentAPIService.GetAllSucessfulTransactions(request, HttpContext, _siteConfig);
                PaginationResponse pageResp = JsonConvert.DeserializeObject<PaginationResponse>(Convert.ToString(resp.Record));
                List<SucessfulTransactionResponse> data = JsonConvert.DeserializeObject<List<SucessfulTransactionResponse>>(Convert.ToString(pageResp.Data));

                foreach (var transaction in data)
                {
                    var order = await _orderItemRepo.GetByID(int.Parse(transaction.orderID));
                    transaction.FromName = order.customerDetail.Name;
                    transaction.FromID = order.customerDetail.ID.ToString();
                    if (transaction.UserID == "BAETOTI")
                    {
                        transaction.ToName = "Baetoti (commission)";
                    }
                    else if (transaction.userType == Shared.Enum.UserType.Provider)
                    {
                        transaction.ToName = order.providerDetail.Name;
                        transaction.ToID = order.providerDetail.ID.ToString();
                    }
                    else if (transaction.userType == Shared.Enum.UserType.Driver)
                    {
                        transaction.ToName = order.driverDetail.Name;
                        transaction.ToID = order.driverDetail.ID.ToString();
                    }

                    transaction.AuthEntry.BuyerName = order.customerDetail.Name;
                    transaction.AuthEntry.ProviderName = order.providerDetail.Name;
                    transaction.AuthEntry.DidItCreateOrder = true;
                    transaction.AuthEntry.ProviderID = order.providerDetail.ID.ToString();
                    transaction.AuthEntry.BuyerID = order.customerDetail.ID.ToString();
                    transaction.OrderStatus = order.orderStatus.OrderStatus;
                }
                pageResp.Data = data;
                return Ok(new SharedResponse(true, 200, "success", pageResp));
            }
            catch (System.Exception ex)
            {

                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }
    }
}
