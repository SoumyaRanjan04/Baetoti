using Baetoti.Core.Entites;
using Microsoft.EntityFrameworkCore;

namespace Baetoti.Infrastructure.Data.Context
{
    public class LoggingDbContext : DbContext
    {
        public LoggingDbContext(DbContextOptions<LoggingDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer(@"");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AppException>(b =>
            {
                b.ToTable(nameof(AppException));
            });
        }

        public DbSet<AppException> AppException { get; set; }

    }

}
