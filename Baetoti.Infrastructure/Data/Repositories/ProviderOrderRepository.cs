using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class ProviderOrderRepository : EFRepository<ProviderOrder>, IProviderOrderRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public ProviderOrderRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProviderOrder> GetByOrderID(long OrderID)
        {
            return await _dbContext.ProviderOrders.Where(x => x.OrderID == OrderID).FirstOrDefaultAsync();
        }

        public async Task<Provider> GetProviderByItemID(long ItemID)
        {
            return await (from i in _dbContext.Items.Where(i => i.ID == ItemID)
                          join p in _dbContext.Providers on i.ProviderID equals p.ID
                          select p).FirstOrDefaultAsync();
        }

        public async Task<Provider> GetProviderByOrderID(long OrderID)
        {
            return await (from po in _dbContext.ProviderOrders.Where(p => p.OrderID == OrderID)
                          join p in _dbContext.Providers on po.ProviderID equals p.ID
                          select p).FirstOrDefaultAsync();
        }

    }
}
