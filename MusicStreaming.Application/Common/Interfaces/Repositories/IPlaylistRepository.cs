using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Repositories
{
    public interface IPlaylistRepository : IGenericRepository<Playlist>
    {
        Task<Playlist> GetByIdAndUserIdAsync(int playlistId, string userId, CancellationToken cancellationToken);
    }
}
