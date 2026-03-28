using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Playlists
{
    public class GetSongsInPlaylistEndpoint : IEndpointHandler
    {
        private readonly ILibraryAdminService _libraryAdminService;
        public GetSongsInPlaylistEndpoint(ILibraryAdminService libraryAdminService) => _libraryAdminService = libraryAdminService;
        public string ActionName => "GetSongsInPlaylist";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            if (!int.TryParse(payload, out int playlistId)) throw new Exception("Invalid Playlist ID");
            return await _libraryAdminService.GetSongsInPlaylistAsync(playlistId, cancellationToken);
        }
    }
}
