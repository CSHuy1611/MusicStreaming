using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Persistence.Configurations
{
    public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
    {
        public void Configure(EntityTypeBuilder<SystemLog> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Action)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.AdminId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(x => x.EntityId)
                .HasMaxLength(100);

            // Tạo Index cho Timestamp để sau này Admin lọc log theo ngày cho nhanh
            builder.HasIndex(x => x.Timestamp);
        }
    }
}
