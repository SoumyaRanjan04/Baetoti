using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Request.Invoice;
using Baetoti.Shared.Response.Invoice;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IInvoiceRepository : IAsyncRepository<Invoice>
    {
        Task<InvoiceResponse> GetInvoice(InvoiceRequest invoiceRequest);

        Task<Invoice> GenerateInvoice(long OrderID, int invoiceType);

    }
}
