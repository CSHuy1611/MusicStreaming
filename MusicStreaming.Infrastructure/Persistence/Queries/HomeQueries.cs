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
    public class HomeQueries : IHomeQueries
    {
        private readonly ApplicationDbContext _context;

        public HomeQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HomeDataVm> GetHomeDataAsync(CancellationToken cancellationToken)
        {
            var playlists = await _context.Playlists
                .AsNoTracking()
                .Where(p => p.IsSystemPlaylist)
                .OrderByDescending(p => p.Id)
                .Take(6)
                .Select(p => new PlaylistDto
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync(cancellationToken);

            var songs = await _context.Songs
                .AsNoTracking()
                .Include(s => s.Singer)
                .OrderByDescending(s => s.ListenCount)
                .Take(12)
                .Select(s => new SongDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    ArtistName = s.Singer.Name,
                    ImageUrl = s.ImageUrl,
                    AudioUrl = s.AudioUrl,
                    Duration = s.Duration,
                    IsVip = s.IsVip
                })
                .ToListAsync(cancellationToken);

            var artists = await _context.Singers
                .AsNoTracking()
                .Take(12)
                .Select(a => new ArtistDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    AvatarUrl = a.AvatarUrl
                })
                .ToListAsync(cancellationToken);

            return new HomeDataVm
            {
                NewPlaylists = playlists,
                TrendingSongs = songs,
                SuggestionArtists = artists
            };
        }
    }
}
