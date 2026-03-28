using Microsoft.EntityFrameworkCore;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Interfaces.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Persistence.Queries
{
    public class LibraryQueries : ILibraryQueries
    {
        private readonly ApplicationDbContext _context;

        public LibraryQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LibraryPlaylistDto>> GetUserPlaylistsAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.Playlists
                .AsNoTracking()
                .Where(p => p.UserId == userId && !p.IsSystemPlaylist)
                .OrderByDescending(p => p.Id)
                .Select(p => new LibraryPlaylistDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = "https://via.placeholder.com/150/282828/FFFFFF?text=Playlist"
                })
                .ToListAsync(cancellationToken);
        }
    }
}
