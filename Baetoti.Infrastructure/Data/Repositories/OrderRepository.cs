using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class OrderRepository : EFRepository<Order>, IOrderRepository
    {
        private readonly BaetotiDbContext _dbContext;

        public OrderRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
