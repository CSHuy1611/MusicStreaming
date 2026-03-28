using Microsoft.EntityFrameworkCore;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Persistence.Queries
{
    public class SingerQueries : ISingerQueries
    {
        private readonly ApplicationDbContext _context;

        public SingerQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<SingerDto>> GetAllSingersAsync(CancellationToken cancellationToken)
        {
            var singers = _context.Singers
                .AsNoTracking()
                .Select(s => new SingerDto
                {
                    Id = s.Id,
                    Name = s.Name,
                })
                .ToListAsync(cancellationToken);
            return singers;
        }

        public async Task<ArtistProfileVm> GetArtistProfileAsync(int singerId, CancellationToken cancellationToken)
        {
            var artistProfile = await _context.Singers
                .AsNoTracking()
                .Where(s => s.Id == singerId)
                .Select(s => new ArtistProfileVm
                {
                    Id = s.Id,
                    Name = s.Name,
                    AvatarUrl = s.AvatarUrl,
                    Bio = s.Bio,
                    Songs = _context.Songs
                        .Where(song => song.SingerId == s.Id)
                        .Select(song => new SongDto
                        {
                            Id = song.Id,
                            Title = song.Title,
                            ArtistName = s.Name, 
                            ImageUrl = song.ImageUrl,
                            AudioUrl = song.AudioUrl,
                            Duration = song.Duration,
                            IsVip = song.IsVip
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return artistProfile;
        }


    }
}
