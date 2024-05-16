using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Request.Invoice;
using Baetoti.Shared.Response.Invoice;
using QRCoder;
using System.Drawing;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Enum;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class InvoiceRepositoy : EFRepository<Invoice>, IInvoiceRepository
    {

        private readonly BaetotiDbContext _dbContext;
        private readonly IConfiguration _config;

        public InvoiceRepositoy(
            BaetotiDbContext dbContext,
            IConfiguration configuration
            ) : base(dbContext)
        {
            _dbContext = dbContext;
            _config = configuration;
        }

        public async Task<Invoice> GenerateInvoice(long OrderID, int invoiceType)
        {
            Invoice invoice = await _dbContext.Invoices.Where(i => i.OrderId == OrderID && i.InvoiceType == invoiceType).FirstOrDefaultAsync();
            if (invoice == null)
            {
                invoice = new Invoice();
                invoice.OrderId = OrderID;
                invoice.InvoiceType = invoiceType;
                invoice.QRCode = Guid.NewGuid().ToString();
                _dbContext.Invoices.Add(invoice);
                _dbContext.SaveChanges();
            }
            return invoice;
        }

        public async Task<InvoiceResponse> GetInvoice(InvoiceRequest invoiceRequest)
        {
            Invoice invoice = _dbContext.Invoices.Where(i => i.OrderId == invoiceRequest.OrderID && i.InvoiceType == invoiceRequest.InvoiceType).FirstOrDefault();
            if (invoice == null)
            {
                invoice = new Invoice();
                invoice.OrderId = invoiceRequest.OrderID;
                invoice.InvoiceType = invoiceRequest.InvoiceType;
                invoice.QRCode = Guid.NewGuid().ToString();
                _dbContext.Invoices.Add(invoice);
                _dbContext.SaveChanges();
            }

            //QRCodeGenerator QrGenerator = new QRCodeGenerator();
            //QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(invoice.QRCode, QRCodeGenerator.ECCLevel.Q);
            //QRCode QrCode = new QRCode(QrCodeInfo);
            //Bitmap QrBitmap = QrCode.GetGraphic(60);
            //byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            //string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));

            InvoiceResponse response;

            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("Default")))
            {
                var param = new DynamicParameters();
                param.Add("@OrderID", invoiceRequest.OrderID);
                using (var m = db.QueryMultiple("[baetoti].[GetInvoice]", param, commandType: CommandType.StoredProcedure))
                {
                    response = await m.ReadFirstOrDefaultAsync<InvoiceResponse>();
                    response.InvoiceType = $"{((InvoiceType)invoiceRequest.InvoiceType).ToString().ToUpper()} INVOICE";
                    //response.QRCode = QrUri;
                    response.QRCode = invoice.QRCode;
                    response.OrderDetails = m.Read<OrderDetails>().ToList();
                }
            }

            return response;
        }

    }
}
