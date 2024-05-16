using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class DriverOrderRepository : EFRepository<DriverOrder>, IDriverOrderRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public DriverOrderRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DriverOrder> GetByOrderID(long OrderID)
        {
            return await _dbContext.DriverOrders.Where(x => x.OrderID == OrderID).FirstOrDefaultAsync();
        }

        public async Task<Driver> GetDriverByOrderID(long OrderID)
        {
            return await (from dor in _dbContext.DriverOrders.Where(d => d.OrderID == OrderID)
                          join d in _dbContext.Drivers on dor.DriverID equals d.ID
                          select d).FirstOrDefaultAsync();
        }

    }
}
