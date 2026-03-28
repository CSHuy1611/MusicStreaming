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
    public class SubscriptionPackageConfiguration : IEntityTypeConfiguration<SubscriptionPackage>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPackage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            // Quan trọng: Định dạng tiền tệ (18 số, 2 số thập phân)
            builder.Property(x => x.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            // Chỉ query gói đang kích hoạt (Tùy chọn, nếu muốn admin thấy gói ẩn thì bỏ dòng này)
            // builder.HasQueryFilter(x => x.IsActive); 
        }
    }
}
