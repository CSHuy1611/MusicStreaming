using Microsoft.EntityFrameworkCore;
using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Interfaces.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Persistence.Queries
{
    public class GenreQueries : IGenreQueries
    {
        private readonly ApplicationDbContext _context;

        public GenreQueries(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task<List<GenreDto>> GetAllGenresAsync(CancellationToken cancellationToken)
        {
            var genres = _context.Genres
                .AsNoTracking()
                .Select(g => new GenreDto
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToListAsync(cancellationToken);
            return genres;
        }
    }
}
