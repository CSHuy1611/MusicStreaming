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
    public class UpdateSingerEndpoint : IEndpointHandler
    {
        private readonly ISingerAdminService _singerAdminService;

        public UpdateSingerEndpoint(ISingerAdminService singerAdminService)
        {
            _singerAdminService = singerAdminService;
        }

        public string ActionName => "UpdateSinger";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            var dto = JsonSerializer.Deserialize<UpdateSingerDto>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (dto == null)
                throw new Exception("Invalid payload for UpdateSinger.");

            return await _singerAdminService.UpdateSingerAsync(dto, cancellationToken);
        }
    }
}
