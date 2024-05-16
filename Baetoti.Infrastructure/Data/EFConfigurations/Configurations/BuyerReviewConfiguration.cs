using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class BuyerReviewConfiguration : IEntityTypeConfiguration<BuyerReview>
	{
		public void Configure(EntityTypeBuilder<BuyerReview> builder)
		{
			builder.ToTable(nameof(BuyerReview), DBSchema.baetoti.ToString());
		}

	}
}
