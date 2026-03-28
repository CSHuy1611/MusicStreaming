using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Playlists
{
    public class GetSystemPlaylistsEndpoint : IEndpointHandler
    {
        private readonly ILibraryAdminService _libraryAdminService;
        public GetSystemPlaylistsEndpoint(ILibraryAdminService libraryAdminService) => _libraryAdminService = libraryAdminService;
        public string ActionName => "GetSystemPlaylists";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            return await _libraryAdminService.GetSystemPlaylistsAsync(cancellationToken);
        }
    }
}
