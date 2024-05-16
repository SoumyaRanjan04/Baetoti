using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class StoreScheduleConfiguration : IEntityTypeConfiguration<StoreSchedule>
    {
        public void Configure(EntityTypeBuilder<StoreSchedule> builder)
        {
            builder.ToTable(nameof(StoreSchedule), DBSchema.baetoti.ToString());
        }
    }
}
