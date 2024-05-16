using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class InstragramTokenRepository : EFRepository<InstragramToken>, IInstragramTokenRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public InstragramTokenRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<InstragramToken> GetByStoreID(long StoreID)
        {
            return await _dbContext.InstragramTokens.Where(t => t.StoreID == StoreID).FirstOrDefaultAsync();
        }

    }
}
