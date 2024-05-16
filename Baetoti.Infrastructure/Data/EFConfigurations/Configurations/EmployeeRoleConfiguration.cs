using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class EmployeeRoleConfiguration : IEntityTypeConfiguration<EmployeeRole>
    {
        public void Configure(EntityTypeBuilder<EmployeeRole> builder)
        {
            builder.ToTable(nameof(EmployeeRole), DBSchema.baetoti.ToString());

        }
    }
}
