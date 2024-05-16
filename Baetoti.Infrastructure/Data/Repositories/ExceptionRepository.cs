using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class ExceptionRepository : IExceptionRepository
    {

        public readonly LoggingDbContext _dbContext;

        public ExceptionRepository(LoggingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AppException> AddAsync(AppException entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

    }

}
