using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Genres
{
    public class DeleteGenreLibEndpoint : IEndpointHandler
    {
        private readonly ILibraryAdminService _libraryAdminService;
        public DeleteGenreLibEndpoint(ILibraryAdminService libraryAdminService) => _libraryAdminService = libraryAdminService;
        public string ActionName => "DeleteGenreLib";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            if (!int.TryParse(payload, out int id)) throw new Exception("Invalid ID");
            var result = await _libraryAdminService.DeleteGenreAsync(id, cancellationToken);
            if (!result) throw new Exception("Không tìm thấy thể loại hoặc đã bị xóa.");
            return result;
        }
    }
}
