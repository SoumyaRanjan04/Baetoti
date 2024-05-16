using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class ProviderBusinessRepository : EFRepository<ProviderBusiness>, IProviderBusinessRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public ProviderBusinessRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProviderBusiness> GetByProviderID(long ProviderID)
        {
            return await _dbContext.ProviderBusiness.Where(x => x.ProviderID == ProviderID).FirstOrDefaultAsync();
        }

    }
}
