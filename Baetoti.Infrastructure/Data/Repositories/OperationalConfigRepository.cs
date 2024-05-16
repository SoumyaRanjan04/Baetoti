using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class OperationalConfigRepository : EFRepository<OperationalConfig>, IOperationalConfigRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public OperationalConfigRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
