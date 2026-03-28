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
    public class PlaylistSongConfiguration : IEntityTypeConfiguration<PlaylistSong>
    {
        public void Configure(EntityTypeBuilder<PlaylistSong> builder)
        {
            // Composite Key (Khóa phức hợp)
            builder.HasKey(x => new { x.PlaylistId, x.SongId });

            builder.HasOne(x => x.Playlist)
                .WithMany(p => p.PlaylistSongs)
                .HasForeignKey(x => x.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Playlist -> Xóa luôn danh sách bài trong đó

            builder.HasOne(x => x.Song)
                .WithMany()
                .HasForeignKey(x => x.SongId)
                .OnDelete(DeleteBehavior.Cascade); // Logic này tùy bạn: Xóa bài hát -> Mất khỏi playlist
        }
    }
}
