using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class CityRepository : EFRepository<City>, ICityRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public CityRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<City>> GetAll(string RegionID)
        {
            return await _dbContext.Cities.Where(c => c.RegionId == RegionID).ToListAsync();
        }

    }
}
