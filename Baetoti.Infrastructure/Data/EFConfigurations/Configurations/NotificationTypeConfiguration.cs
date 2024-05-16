using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationType = Baetoti.Core.Entites.NotificationType;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class NotificationTypeConfiguration : IEntityTypeConfiguration<NotificationType>
    {
        public void Configure(EntityTypeBuilder<NotificationType> builder)
        {
            builder.ToTable(nameof(NotificationType), DBSchema.baetoti.ToString());
        }

    }
}
