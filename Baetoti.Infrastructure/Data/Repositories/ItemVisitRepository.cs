using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class ItemVisitRepository : EFRepository<ItemVisit>, IItemVisitRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public ItemVisitRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ItemVisit> LogItemVisit(ItemVisit itemVisit)
        {
            ItemVisit IsAlreadyExist = await _dbContext.ItemVisits.Where(i =>i.UserID == itemVisit.UserID && i.ItemID == itemVisit.ItemID).FirstOrDefaultAsync();
           
            if (IsAlreadyExist == null)
            {
                await _dbContext.ItemVisits.AddAsync(itemVisit);
                _dbContext.SaveChanges();
            }
            return IsAlreadyExist;
        }

    }
}
