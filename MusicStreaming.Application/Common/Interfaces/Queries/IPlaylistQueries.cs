using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Queries
{
    public interface IPlaylistQueries
    {
        Task<PlaylistDetailVm> GetPlaylistDetailAsync(int playlistId, string userId, CancellationToken cancellationToken);
    }
}
