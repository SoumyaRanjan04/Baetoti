using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class ItemImageConfiguration : IEntityTypeConfiguration<ItemImage>
	{
		public void Configure(EntityTypeBuilder<ItemImage> builder)
		{
			builder.ToTable(nameof(ItemImage), DBSchema.baetoti.ToString());
		}

	}
}
