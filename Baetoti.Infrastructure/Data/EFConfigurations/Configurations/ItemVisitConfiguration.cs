using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class ItemVisitConfiguration : IEntityTypeConfiguration<ItemVisit>
    {
        public void Configure(EntityTypeBuilder<ItemVisit> builder)
        {
            builder.ToTable(nameof(ItemVisit), DBSchema.baetoti.ToString());
        }

    }
}
