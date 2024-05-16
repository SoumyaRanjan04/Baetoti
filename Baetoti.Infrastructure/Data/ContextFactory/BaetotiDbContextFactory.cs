using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.ContextFactory.Base;
using Microsoft.EntityFrameworkCore;

namespace Baetoti.Infrastructure.Data.ContextFactory
{
    public class BaetotiDbContextFactory : DesignTimeDbContextFactoryBase<BaetotiDbContext>
    {
        protected override BaetotiDbContext CreateNewInstance(DbContextOptions<BaetotiDbContext> options)
        {
            return new BaetotiDbContext(options);
        }
    }
}
