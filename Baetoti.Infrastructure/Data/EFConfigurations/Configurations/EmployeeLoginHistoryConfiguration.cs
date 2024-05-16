using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class EmployeeLoginHistoryConfiguration : IEntityTypeConfiguration<EmployeeLoginHistory>
    {
        public void Configure(EntityTypeBuilder<EmployeeLoginHistory> builder)
        {
            builder.ToTable(nameof(EmployeeLoginHistory), DBSchema.baetoti.ToString());
        }

    }
}
