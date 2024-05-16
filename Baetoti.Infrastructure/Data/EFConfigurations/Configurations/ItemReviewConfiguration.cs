using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    class ItemReviewConfiguration : IEntityTypeConfiguration<ItemReview>
    {
        public void Configure(EntityTypeBuilder<ItemReview> builder)
        {
            builder.ToTable(nameof(ItemReview), DBSchema.baetoti.ToString());
        }

    }
}
