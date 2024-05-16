using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class ChangeItemConfiguration : IEntityTypeConfiguration<ChangeItem>
    {
        public void Configure(EntityTypeBuilder<ChangeItem> builder)
        {
            builder.ToTable(nameof(ChangeItem), DBSchema.baetoti.ToString());
        }
    }
}
