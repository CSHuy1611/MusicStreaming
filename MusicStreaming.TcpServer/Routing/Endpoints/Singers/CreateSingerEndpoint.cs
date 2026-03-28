using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Singers
{
    public class CreateSingerEndpoint : IEndpointHandler
    {
        private readonly ISingerAdminService _singerAdminService;

        public CreateSingerEndpoint(ISingerAdminService singerAdminService)
        {
            _singerAdminService = singerAdminService;
        }

        public string ActionName => "CreateSinger";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {

            var dto = JsonSerializer.Deserialize<CreateSingerDto>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (dto == null)
                throw new Exception("Invalid payload for CreateSinger.");

            return await _singerAdminService.CreateSingerAsync(dto, cancellationToken);
        }
    }
}
