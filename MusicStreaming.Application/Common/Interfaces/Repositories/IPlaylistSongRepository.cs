using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Repositories
{
    public interface IPlaylistSongRepository : IGenericRepository<PlaylistSong>
    {
        Task<bool> IsSongExistInPlaylistAsync(int playlistId, int songId, CancellationToken cancellationToken);
    }
}
