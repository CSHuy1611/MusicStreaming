using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Playlists
{
    public class DeleteSystemPlaylistEndpoint : IEndpointHandler
    {
        private readonly ILibraryAdminService _libraryAdminService;
        public DeleteSystemPlaylistEndpoint(ILibraryAdminService libraryAdminService) => _libraryAdminService = libraryAdminService;
        public string ActionName => "DeleteSystemPlaylist";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            if (!int.TryParse(payload, out int id)) throw new Exception("Invalid ID");
            var result = await _libraryAdminService.DeleteSystemPlaylistAsync(id, cancellationToken);
            if (!result) throw new Exception("Không tìm thấy Playlist hoặc đã bị xóa.");
            return result;
        }
    }
}
