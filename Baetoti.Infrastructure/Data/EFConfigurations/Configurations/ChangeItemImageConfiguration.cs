using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class ChangeItemImageConfiguration : IEntityTypeConfiguration<ChangeItemImage>
	{
		public void Configure(EntityTypeBuilder<ChangeItemImage> builder)
		{
			builder.ToTable(nameof(ChangeItemImage), DBSchema.baetoti.ToString());
		}

	}
}
