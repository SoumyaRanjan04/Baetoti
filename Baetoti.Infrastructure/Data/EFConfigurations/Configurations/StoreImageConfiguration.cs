using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class StoreImageConfiguration : IEntityTypeConfiguration<StoreImage>
	{
		public void Configure(EntityTypeBuilder<StoreImage> builder)
		{
			builder.ToTable(nameof(StoreImage), DBSchema.baetoti.ToString());
		}

	}
}
