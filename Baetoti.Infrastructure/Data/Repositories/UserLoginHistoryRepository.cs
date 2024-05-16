using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class UserLoginHistoryRepository : EFRepository<UserLoginHistory>, IUserLoginHistoryRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public UserLoginHistoryRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserLoginHistory> GetByUserID(long UserID)
        {
            return await _dbContext.UserLoginHistory
                .Where(x => x.UserID == UserID).OrderByDescending(x => x.ID).FirstOrDefaultAsync();
        }

    }
}
