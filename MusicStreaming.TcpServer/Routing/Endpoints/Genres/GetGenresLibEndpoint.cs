using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Genres
{
    public class GetGenresLibEndpoint : IEndpointHandler
    {
        private readonly ILibraryAdminService _libraryAdminService;
        public GetGenresLibEndpoint(ILibraryAdminService libraryAdminService) => _libraryAdminService = libraryAdminService;
        public string ActionName => "GetGenresLib";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            return await _libraryAdminService.GetAllGenresAsync(cancellationToken);
        }
    }
}
