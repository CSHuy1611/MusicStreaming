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
    public class SongQueries : ISongQueries
    {
        private readonly ApplicationDbContext _context;

        public SongQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<SongDto>> GetAllSongsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            // 1. Tạo query 
            var query = _context.Songs
                .AsNoTracking()
                .Where(s => !s.IsDeleted);

            // 2. Đếm tổng số bản ghi 
            int totalCount = await query.CountAsync(cancellationToken);

            // 3. Phân trang
            var items = await query
                .OrderByDescending(s => s.Id) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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

            // 4. Đóng gói kết quả trả về
            return new PagedResult<SongDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
