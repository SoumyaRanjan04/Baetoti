using Baetoti.Core.Entites;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Infrastructure.Data.EFConfigurations.Configurations
{
    class ChangeItemTagConfiguration : IEntityTypeConfiguration<ChangeItemTag>
    {
        public void Configure(EntityTypeBuilder<ChangeItemTag> builder)
        {
            builder.ToTable(nameof(ChangeItemTag), DBSchema.baetoti.ToString());
        }
    }
}
