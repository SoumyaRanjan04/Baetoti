using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class ReportedOrderConfiguration : IEntityTypeConfiguration<ReportedOrder>
	{
		public void Configure(EntityTypeBuilder<ReportedOrder> builder)
		{
			builder.ToTable(nameof(ReportedOrder), DBSchema.baetoti.ToString());
		}

	}
}
