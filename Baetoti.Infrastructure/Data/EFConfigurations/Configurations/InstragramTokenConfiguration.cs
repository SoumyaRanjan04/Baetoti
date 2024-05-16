using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class InstragramTokenConfiguration : IEntityTypeConfiguration<InstragramToken>
    {
        public void Configure(EntityTypeBuilder<InstragramToken> builder)
        {
            builder.ToTable(nameof(InstragramToken), DBSchema.baetoti.ToString());
        }

    }
}
