using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
	public class AccountVisitConfiguration : IEntityTypeConfiguration<AccountVisit>
	{
		public void Configure(EntityTypeBuilder<AccountVisit> builder)
		{
			builder.ToTable(nameof(AccountVisit), DBSchema.baetoti.ToString());
		}

	}
}
