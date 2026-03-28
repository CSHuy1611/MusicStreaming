using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Playlists
{
    public class RemoveSongFromPlaylistEndpoint : IEndpointHandler
    {
        private readonly ILibraryAdminService _libraryAdminService;
        public RemoveSongFromPlaylistEndpoint(ILibraryAdminService libraryAdminService) => _libraryAdminService = libraryAdminService;
        public string ActionName => "RemoveSongFromPlaylist";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            var dto = JsonSerializer.Deserialize<RemoveSongFromPlaylistDto>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (dto == null) throw new Exception("Invalid payload");
            return await _libraryAdminService.RemoveSongFromPlaylistAsync(dto, cancellationToken);
        }
    }
}
