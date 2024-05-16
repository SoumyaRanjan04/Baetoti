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
    public class OTPRepository : EFRepository<OTP>, IOTPRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public OTPRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<OTP> GetByUserIdAsync(long UserID)
        {
            return _dbContext.OTPs.Where(x =>
                                x.UserID == UserID &&
                                x.OTPStatus == (int)OTPStatus.Active
                                ).OrderByDescending(x => x.OTPGeneratedAt).FirstOrDefaultAsync();
        }

    }
}
