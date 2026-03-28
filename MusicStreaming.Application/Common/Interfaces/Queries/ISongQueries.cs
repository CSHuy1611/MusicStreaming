using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Queries
{
    public interface ISongQueries
    {
        Task<PagedResult<SongDto>> GetAllSongsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
