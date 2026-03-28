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
    public class SearchQueries : ISearchQueries
    {
        private readonly ApplicationDbContext _context;

        public SearchQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SearchVm> SearchSongsAsync(string keyword, CancellationToken cancellationToken)
        {
            var query = keyword.ToLower().Trim();

            var songs = await _context.Songs
                .AsNoTracking()
                .Where(s =>
                    EF.Functions.Like(s.Title, $"%{query}%") ||
                    EF.Functions.Like(s.Singer.Name, $"%{query}%"))
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

            return new SearchVm
            {
                Keyword = keyword,
                Songs = songs
            };
        }
    }
}
