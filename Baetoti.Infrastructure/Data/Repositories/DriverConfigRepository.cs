using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.DriverConfig;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class DriverConfigRepository : EFRepository<DriverConfig>, IDriverConfigRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public DriverConfigRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DriverConfigResponse>> GetAllConfig()
        {
            return await (from d in _dbContext.DriverConfigs
                          join e in _dbContext.Employee on d.CreatedBy equals (int?)e.ID
                          select new DriverConfigResponse
                          {
                              AdditionalKM = d.AdditionalKM,
                              ByEmployee = e.FirstName,
                              AdditionalRatePerKM = d.AdditionalRatePerKM,
                              CreatedAt = d.CreatedAt,
                              Currency = d.Currency,
                              DriverComission = d.DriverComission,
                              FromKM = d.FromKM,
                              ID = d.ID,
                              MarkAsDeleted = d.MarkAsDeleted,
                              MaximumDistance = d.MaximumDistance,
                              ProviderComission = d.ProviderComission,
                              RatePerKM = d.RatePerKM,
                              ServiceFee = d.ServiceFee,
                              ServiceFeeFixed = d.ServiceFeeFixed,
                              ToKM = d.ToKM
                          }).OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

    }
}
