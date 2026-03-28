using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Genres
{
    public class CreateGenreLibEndpoint : IEndpointHandler
    {
        private readonly ILibraryAdminService _libraryAdminService;
        public CreateGenreLibEndpoint(ILibraryAdminService libraryAdminService) => _libraryAdminService = libraryAdminService;
        public string ActionName => "CreateGenreLib";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            var dto = JsonSerializer.Deserialize<CreateGenreDto>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (dto == null) throw new Exception("Invalid payload");
            return await _libraryAdminService.CreateGenreAsync(dto, cancellationToken);
        }
    }
}
