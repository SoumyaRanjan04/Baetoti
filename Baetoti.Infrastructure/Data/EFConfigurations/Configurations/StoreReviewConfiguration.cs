using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class StoreReviewConfiguration : IEntityTypeConfiguration<StoreReview>
	{
		public void Configure(EntityTypeBuilder<StoreReview> builder)
		{
			builder.ToTable(nameof(StoreReview), DBSchema.baetoti.ToString());
		}

	}
}
