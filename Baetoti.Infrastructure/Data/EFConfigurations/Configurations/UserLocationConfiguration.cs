using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class UserLocationConfiguration : IEntityTypeConfiguration<UserLocation>
	{
		public void Configure(EntityTypeBuilder<UserLocation> builder)
		{
			builder.ToTable(nameof(UserLocation), DBSchema.baetoti.ToString());
		}
	}
}
