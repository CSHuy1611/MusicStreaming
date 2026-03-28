using MusicStreaming.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Queries
{
    public interface ILibraryQueries
    {
        Task<List<LibraryPlaylistDto>> GetUserPlaylistsAsync(string userId, CancellationToken cancellationToken);
    }
}
