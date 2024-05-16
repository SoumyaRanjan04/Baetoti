using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Enum;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class AccountVisitRepository : EFRepository<AccountVisit>, IAccountVisitRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public AccountVisitRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AccountVisit> LogAccountVisit(AccountVisit accountVisit)
        {
            if (accountVisit.UserType == (int)UserType.Provider)
            {
                AccountVisit IsAlreadyExist = _dbContext.AccountVisits
                .Where(x => x.ProviderID == accountVisit.ProviderID &&
                x.UserID == accountVisit.UserID &&
                x.UserType == accountVisit.UserType &&
                x.FirstVisitDate.Date == accountVisit.FirstVisitDate.Date).FirstOrDefault();
                if (IsAlreadyExist == null)
                {
                    await _dbContext.AccountVisits.AddAsync(accountVisit);
                    _dbContext.SaveChanges();
                }
                return IsAlreadyExist;
            }
            if (accountVisit.UserType == (int)UserType.Driver)
            {
                AccountVisit IsAlreadyExist = _dbContext.AccountVisits
                .Where(x => x.DriverID == accountVisit.DriverID &&
                x.UserID == accountVisit.UserID &&
                x.UserType == accountVisit.UserType &&
                x.FirstVisitDate.Date == accountVisit.FirstVisitDate.Date).FirstOrDefault();
                if (IsAlreadyExist == null)
                {
                    await _dbContext.AccountVisits.AddAsync(accountVisit);
                    _dbContext.SaveChanges();
                }
                return IsAlreadyExist;
            }
            return new AccountVisit();
        }

    }
}
