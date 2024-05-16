using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Transaction;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.Transaction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface ITransactionRepository : IAsyncRepository<Transaction>
    {
        Task<List<AllTransactions>> GetAll();

        Task<PaginationResponse> GetFitered(TransactionFilterRequest request);

        Task<TransactionResponseByID> GetByID(long Id, int UserType);

        Task<Transaction> GetByOrderID(long OrderID);

    }
}
