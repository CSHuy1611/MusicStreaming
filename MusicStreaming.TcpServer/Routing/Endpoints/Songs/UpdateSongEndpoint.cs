using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Songs
{
    public class UpdateSongEndpoint : IEndpointHandler
    {
        private readonly ISongAdminService _songAdminService;

        public UpdateSongEndpoint(ISongAdminService songAdminService)
        {
            _songAdminService = songAdminService;
        }

        public string ActionName => "UpdateSong";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            var dto = JsonSerializer.Deserialize<UpdateSongDto>(payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dto == null) 
                throw new ArgumentException("Payload is null or invalid.");

            var result = await _songAdminService.UpdateSongAsync(dto, cancellationToken);
            return result;
        }
    }
}
