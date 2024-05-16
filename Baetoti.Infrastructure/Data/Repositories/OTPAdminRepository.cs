using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class OTPAdminRepository : EFRepository<OTPAdmin>, IOTPAdminRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public OTPAdminRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OTPAdmin> GetByEmployeeIdAsync(long EmployeeID)
        {
            return await _dbContext.OTPAdmins.Where(x =>
                                x.EmployeeID == EmployeeID &&
                                x.OTPStatus == (int)OTPStatus.Active
                                ).OrderByDescending(x => x.OTPGeneratedAt).FirstOrDefaultAsync();
        }

    }
}
