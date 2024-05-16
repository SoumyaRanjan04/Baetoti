using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.Transaction;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Baetoti.Shared.Enum;
using System;
using Baetoti.Shared.Request.Transaction;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Request.Shared;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class TransactionRepository : EFRepository<Transaction>, ITransactionRepository
    {

        private readonly BaetotiDbContext _dbContext;
        private readonly IConfiguration _config;
        private readonly IInvoiceRepository _invoiceRepository;

        public TransactionRepository(
            BaetotiDbContext dbContext,
            IInvoiceRepository invoiceRepository,
            IConfiguration configuration
            ) : base(dbContext)
        {
            _dbContext = dbContext;
            _config = configuration;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<List<AllTransactions>> GetAll()
        {
            return await (from t in _dbContext.Transactions
                          join u in _dbContext.Users on t.UserID equals u.ID
                          join po in _dbContext.ProviderOrders on t.OrderID equals po.OrderID
                          join p in _dbContext.Providers on po.ProviderID equals p.ID
                          join pu in _dbContext.Users on p.UserID equals pu.ID
                          select new AllTransactions
                          {
                              TransactionID = t.ID,
                              FromUserID = u.ID,
                              ToUserID = pu.ID,
                              OrderID = t.OrderID,
                              TransactionAmount = t.Amount,
                              TransactionFrom = $"{u.FirstName} {u.LastName}",
                              TransactionTo = $"{pu.FirstName} {pu.LastName}",
                              TransactionFor = "",
                              TransactionStatus = Convert.ToString((TransactionStatus)t.Status),
                              PaymentType = Convert.ToString((TransactionType)t.TransactionType),
                              TransactionTime = t.TransactionTime
                          }).ToListAsync();
        }

        public async Task<PaginationResponse> GetFitered(TransactionFilterRequest request)
        {
            PaginationResponse response = new PaginationResponse();
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("Default")))
            {
                var param = new DynamicParameters();
                param.Add("@PageNumber", request.PageNumber);
                param.Add("@PageSize", request.PageSize);
                param.Add("@StartDate", request.DateRange.StartDate);
                param.Add("@EndDate", request.DateRange.EndDate);
                param.Add("@CountryID", request.CountryID);
                param.Add("@RegionID", request.RegionID);
                param.Add("@CityID", request.CityID);
                param.Add("@Gender", request.Gender);
                param.Add("@CategoryID", request.CategoryID);
                param.Add("@SubCategoryID", request.SubCategoryID);
                param.Add("@OrderStatus", request.OrderStatus);
                param.Add("@PaymentType", request.PaymentType);
                using (var m = db.QueryMultiple("[baetoti].[GetAllTransactions]", param, commandType: CommandType.StoredProcedure))
                {
                    var data = m.Read<AllTransactions>().ToList();
                    var totalRecords = await m.ReadFirstOrDefaultAsync<int>();
                    var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                    response.CurrentPage = request.PageNumber;
                    response.TotalPages = totalPages;
                    response.PageSize = request.PageSize;
                    response.TotalCount = totalRecords;
                    response.Data = data;
                }
            }
            return response;
        }

        public async Task<TransactionResponseByID> GetByID(long Id, int UserType)
        {
            TransactionResponseByID response;
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("Default")))
            {
                var param = new DynamicParameters();
                param.Add("@ID", Id);
                param.Add("@UserType", UserType);
                using (var m = db.QueryMultiple("[baetoti].[GetAllTransactions]", param, commandType: CommandType.StoredProcedure))
                {
                    response = await m.ReadFirstOrDefaultAsync<TransactionResponseByID>();
                }
            }

            response.ProviderInvoice = (await _invoiceRepository.GenerateInvoice(response.OrderID, (int)InvoiceType.Provider)).QRCode;
            response.DriverInvoice = (await _invoiceRepository.GenerateInvoice(response.OrderID, (int)InvoiceType.Driver)).QRCode;
            response.UserInvoice = (await _invoiceRepository.GenerateInvoice(response.OrderID, (int)InvoiceType.User)).QRCode;

            return response;
        }
        public async Task<Transaction> GetByOrderID(long OrderID)
        {
            return await _dbContext.Transactions.Where(t => t.OrderID == OrderID).FirstOrDefaultAsync();
        }
    }
}
