using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class FavouriteDriverConfiguration : IEntityTypeConfiguration<FavouriteDriver>
	{
		public void Configure(EntityTypeBuilder<FavouriteDriver> builder)
		{
			builder.ToTable(nameof(FavouriteDriver), DBSchema.baetoti.ToString());
		}

	}
}
