using Microsoft.EntityFrameworkCore;

namespace Baetoti.Infrastructure.Data.Context
{
    public class DapperDbContext : DbContext
    {
        public DapperDbContext() { }
        public DapperDbContext(DbContextOptions<DapperDbContext> options) : base(options) { }
    }
}
