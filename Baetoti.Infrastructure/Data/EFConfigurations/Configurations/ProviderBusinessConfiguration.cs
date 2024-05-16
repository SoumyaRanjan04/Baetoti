using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{	
	public class ProviderBusinessConfiguration : IEntityTypeConfiguration<ProviderBusiness>
	{
		public void Configure(EntityTypeBuilder<ProviderBusiness> builder)
		{
			builder.ToTable(nameof(ProviderBusiness), DBSchema.baetoti.ToString());
		}

	}
}
