using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class SiteConfigConfiguration : IEntityTypeConfiguration<SiteConfig>
	{
		public void Configure(EntityTypeBuilder<SiteConfig> builder)
		{
			builder.ToTable(nameof(SiteConfig), DBSchema.baetoti.ToString());
		}

	}
}
