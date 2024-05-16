using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class DriverReviewConfiguration : IEntityTypeConfiguration<DriverReview>
	{
		public void Configure(EntityTypeBuilder<DriverReview> builder)
		{
			builder.ToTable(nameof(DriverReview), DBSchema.baetoti.ToString());
		}

	}
}
