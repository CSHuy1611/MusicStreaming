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
    public class UpdateSystemPlaylistEndpoint : IEndpointHandler
    {
        private readonly ILibraryAdminService _libraryAdminService;
        public UpdateSystemPlaylistEndpoint(ILibraryAdminService libraryAdminService) => _libraryAdminService = libraryAdminService;
        public string ActionName => "UpdateSystemPlaylist";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            var dto = JsonSerializer.Deserialize<UpdateSystemPlaylistDto>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (dto == null) throw new Exception("Invalid payload");
            return await _libraryAdminService.UpdateSystemPlaylistAsync(dto, cancellationToken);
        }
    }
}
