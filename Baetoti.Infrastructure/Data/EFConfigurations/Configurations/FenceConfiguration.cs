using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class FenceConfiguration : IEntityTypeConfiguration<Fence>
	{
		public void Configure(EntityTypeBuilder<Fence> builder)
		{
			builder.ToTable(nameof(Fence), DBSchema.baetoti.ToString());
		}

	}
}
