using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class FavouriteStoreConfiguration : IEntityTypeConfiguration<FavouriteStore>
	{
		public void Configure(EntityTypeBuilder<FavouriteStore> builder)
		{
			builder.ToTable(nameof(FavouriteStore), DBSchema.baetoti.ToString());
		}

	}
}
