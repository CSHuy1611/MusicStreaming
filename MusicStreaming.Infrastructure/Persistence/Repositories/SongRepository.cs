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
    public class SongRepository : GenericRepository<Song>, ISongRepository
    {
        private readonly ApplicationDbContext _context;

        public SongRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> IncrementListenCountAsync(int songId, CancellationToken cancellationToken = default)
        {
            var affectedRows = await _context.Songs
                .Where(s => s.Id == songId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.ListenCount, s => s.ListenCount + 1),
                    cancellationToken);

            return affectedRows > 0;
        }

        public async Task<IEnumerable<Song>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Songs
                .Include(s => s.Singer)
                .Include(s => s.Genre)
                .ToListAsync(cancellationToken);
        }
    }
}
