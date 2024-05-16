using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class OTPAdminConfiguration : IEntityTypeConfiguration<OTPAdmin>
    {
        public void Configure(EntityTypeBuilder<OTPAdmin> builder)
        {
            builder.ToTable(nameof(OTPAdmin), DBSchema.baetoti.ToString());
        }

    }
}
