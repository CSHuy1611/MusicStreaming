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
    public class PlaylistRepository : GenericRepository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Playlist> GetByIdAndUserIdAsync(int playlistId, string userId, CancellationToken cancellationToken)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.Id == playlistId && p.UserId == userId, cancellationToken);
        }
    }
}
