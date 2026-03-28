using Microsoft.EntityFrameworkCore;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Persistence.Repositories
{
    public class PlaylistSongRepository : GenericRepository<PlaylistSong>, IPlaylistSongRepository
    {
        public PlaylistSongRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsSongExistInPlaylistAsync(int playlistId, int songId, CancellationToken cancellationToken)
        {
            return await _dbSet.AnyAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId, cancellationToken);
        }
    }
}
