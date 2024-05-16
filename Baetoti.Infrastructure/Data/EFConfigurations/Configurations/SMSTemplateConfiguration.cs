using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class SMSTemplateConfiguration : IEntityTypeConfiguration<SMSTemplate>
    {
        public void Configure(EntityTypeBuilder<SMSTemplate> builder)
        {
            builder.ToTable(nameof(SMSTemplate), DBSchema.baetoti.ToString());
        }

    }
}
