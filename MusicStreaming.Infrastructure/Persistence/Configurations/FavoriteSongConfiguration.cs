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
    public class FavoriteSongConfiguration : IEntityTypeConfiguration<FavoriteSong>
    {
        public void Configure(EntityTypeBuilder<FavoriteSong> builder)
        {
            // Composite Key
            builder.HasKey(x => new { x.UserId, x.SongId });

            builder.Property(x => x.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.HasOne(x => x.Song)
                .WithMany()
                .HasForeignKey(x => x.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
