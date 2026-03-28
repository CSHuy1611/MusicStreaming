using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Singers
{
    public class DeleteSingerEndpoint : IEndpointHandler
    {
        private readonly ISingerAdminService _singerAdminService;

        public DeleteSingerEndpoint(ISingerAdminService singerAdminService)
        {
            _singerAdminService = singerAdminService;
        }

        public string ActionName => "DeleteSinger";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {

            if (!int.TryParse(payload, out int id))
            {
                throw new Exception("Invalid payload for DeleteSinger. Expected an integer ID.");
            }

            var result = await _singerAdminService.DeleteSingerAsync(id, cancellationToken);

            if (!result)
                throw new Exception($"Singer with Id {id} not found or already deleted.");

            return result;
        }
    }
}
