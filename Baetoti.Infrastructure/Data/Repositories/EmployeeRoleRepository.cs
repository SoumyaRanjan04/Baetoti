using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class EmployeeRoleRepository : EFRepository<EmployeeRole>, IEmployeeRoleRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public EmployeeRoleRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        async Task<EmployeeRole> IEmployeeRoleRepository.GetByUserId(long EmployeeId)
        {
            return await _dbContext.EmployeeRoles.Where(x => x.EmployeeId == EmployeeId).FirstOrDefaultAsync();
        }

    }
}
