using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Songs
{
    public class CreateSongEndpoint : IEndpointHandler
    {
        private readonly ISongAdminService _songAdminService;

        public CreateSongEndpoint(ISongAdminService songAdminService)
        {
            _songAdminService = songAdminService;
        }

        public string ActionName => "CreateSong";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            var dto = JsonSerializer.Deserialize<CreateSongDto>(payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dto == null)
                throw new ArgumentException("Payload is null or invalid.");

            var result = await _songAdminService.CreateSongAsync(dto, cancellationToken);
            return result;
        }
    }
}
