using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
	{
		public void Configure(EntityTypeBuilder<Promotion> builder)
		{
			builder.ToTable(nameof(Promotion), DBSchema.baetoti.ToString());
		}
	}
}
