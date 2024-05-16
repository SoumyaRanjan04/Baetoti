using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    public class StoreTagConfiguration : IEntityTypeConfiguration<StoreTag>
    {
        public void Configure(EntityTypeBuilder<StoreTag> builder)
        {
            builder.ToTable(nameof(StoreTag), DBSchema.baetoti.ToString());
        }
    }
}
