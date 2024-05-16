using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class ReportedStoreConfiguration : IEntityTypeConfiguration<ReportedStore>
	{
		public void Configure(EntityTypeBuilder<ReportedStore> builder)
		{
			builder.ToTable(nameof(ReportedStore), DBSchema.baetoti.ToString());
		}

	}
}
