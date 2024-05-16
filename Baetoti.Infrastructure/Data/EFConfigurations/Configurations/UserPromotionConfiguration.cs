using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class UserPromotionConfiguration : IEntityTypeConfiguration<UserPromotion>
	{
		public void Configure(EntityTypeBuilder<UserPromotion> builder)
		{
			builder.ToTable(nameof(UserPromotion), DBSchema.baetoti.ToString());
		}

	}
}
