using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStreaming.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Persistence.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // IdentityUser đã có cấu hình sẵn, ta chỉ cấu hình thêm các trường mới
            builder.Property(x => x.FullName)
                .HasMaxLength(200);

            // Các trường bool (IsPremium) mặc định là bit, không cần config thêm
        }
    }
}
