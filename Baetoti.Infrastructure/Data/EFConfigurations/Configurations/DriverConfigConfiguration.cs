using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class DriverConfigConfiguration : IEntityTypeConfiguration<DriverConfig>
    {
        public void Configure(EntityTypeBuilder<DriverConfig> builder)
        {
            builder.ToTable(nameof(DriverConfig), DBSchema.baetoti.ToString());
        }
    }
}
