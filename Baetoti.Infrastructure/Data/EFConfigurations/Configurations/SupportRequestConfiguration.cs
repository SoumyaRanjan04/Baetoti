using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class SupportRequestConfiguration : IEntityTypeConfiguration<SupportRequest>
	{
		public void Configure(EntityTypeBuilder<SupportRequest> builder)
		{
			builder.ToTable(nameof(SupportRequest), DBSchema.baetoti.ToString());
		}

	}
}
