using Baetoti.Shared.Response.Category;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.API.Controllers.Base;
using Baetoti.Shared.Request.Invoice;
using AutoMapper;
using Baetoti.Core.Interface.Repositories;

namespace Baetoti.API.Controllers
{
    public class InvoiceController : ApiBaseController
    {

        public readonly IInvoiceRepository _invoiceRepository;

        public InvoiceController(
            IInvoiceRepository invoiceRepository
            )
        {
            _invoiceRepository = invoiceRepository;
        }

        [HttpPost("GetInvoice")]
        public async Task<IActionResult> GetInvoice([FromBody] InvoiceRequest invoiceRequest)
        {
            try
            {
                var invoiceResponse = await _invoiceRepository.GetInvoice(invoiceRequest);
                return Ok(new SharedResponse(true, 200, "", invoiceResponse));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
