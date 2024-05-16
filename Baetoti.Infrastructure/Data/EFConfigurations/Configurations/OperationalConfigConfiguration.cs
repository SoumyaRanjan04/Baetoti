using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class OperationalConfigConfiguration : IEntityTypeConfiguration<OperationalConfig>
    {
        public void Configure(EntityTypeBuilder<OperationalConfig> builder)
        {
            builder.ToTable(nameof(OperationalConfig), DBSchema.baetoti.ToString());
        }
    }
}
