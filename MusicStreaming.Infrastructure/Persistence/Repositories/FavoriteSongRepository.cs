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
    public class FavoriteSongRepository : GenericRepository<FavoriteSong>, IFavoriteSongRepository
    {
        public FavoriteSongRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<FavoriteSong> GetByUserAndSongAsync(string userId, int songId, CancellationToken cancellationToken)
        {
            return await _dbSet.FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == songId, cancellationToken);
        }
    }
}
