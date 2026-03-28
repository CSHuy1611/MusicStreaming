using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Repositories
{
    public interface ISongRepository : IGenericRepository<Song>
    {
        Task<bool> IncrementListenCountAsync(int songId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Song>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    }
}
