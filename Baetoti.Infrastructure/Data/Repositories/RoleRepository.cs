using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class RoleRepository : EFRepository<Roles>, IRoleRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public RoleRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
