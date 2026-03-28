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
    public class SongConfiguration : IEntityTypeConfiguration<Song>
    {
        public void Configure(EntityTypeBuilder<Song> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasIndex(x => x.Title)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.Property(x => x.AudioUrl)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            // Cấu hình quan hệ
            builder.HasOne(x => x.Singer)
                .WithMany(s => s.Songs)
                .HasForeignKey(x => x.SingerId)
                .OnDelete(DeleteBehavior.Restrict); // Xóa ca sĩ không làm mất bài hát (giữ data toàn vẹn)

            builder.HasOne(x => x.Genre)
                .WithMany(g => g.Songs)
                .HasForeignKey(x => x.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
