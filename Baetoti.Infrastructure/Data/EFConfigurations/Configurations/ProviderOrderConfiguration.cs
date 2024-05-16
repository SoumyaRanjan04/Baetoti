using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class ProviderOrderConfiguration : IEntityTypeConfiguration<ProviderOrder>
    {
        public void Configure(EntityTypeBuilder<ProviderOrder> builder)
        {
            builder.ToTable(nameof(ProviderOrder), DBSchema.baetoti.ToString());
        }
    }
}
