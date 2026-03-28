using Microsoft.EntityFrameworkCore;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Persistence.Queries
{
    public class FavoriteQueries : IFavoriteQueries
    {
        private readonly ApplicationDbContext _context;

        public FavoriteQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FavoriteSongsVm> GetUserFavoriteSongsAsync(string userId, CancellationToken cancellationToken)
        {
            var songs = await _context.FavoriteSongs
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.LikedDate)
                .Select(f => new SongDto
                {
                    Id = f.Song.Id,
                    Title = f.Song.Title,
                    ArtistName = f.Song.Singer.Name,
                    ImageUrl = f.Song.ImageUrl,
                    AudioUrl = f.Song.AudioUrl,
                    Duration = f.Song.Duration,
                    IsVip = f.Song.IsVip
                })
                .ToListAsync(cancellationToken);

            return new FavoriteSongsVm
            {
                TotalCount = songs.Count,
                Songs = songs
            };
        }

        public async Task<bool> CheckIsFavoriteAsync(string userId, int songId, CancellationToken cancellationToken)
        {
            return await _context.FavoriteSongs.AnyAsync(f => f.UserId == userId && f.SongId == songId, cancellationToken);
        }
    }
}
