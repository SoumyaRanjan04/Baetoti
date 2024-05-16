using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    class ItemTagConfiguration : IEntityTypeConfiguration<ItemTag>
    {
        public void Configure(EntityTypeBuilder<ItemTag> builder)
        {
            builder.ToTable(nameof(ItemTag), DBSchema.baetoti.ToString());
        }

    }
}
