using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Singers
{
    public class GetSingersEndpoint : IEndpointHandler
    {
        private readonly ISingerAdminService _singerAdminService;

        public GetSingersEndpoint(ISingerAdminService singerAdminService)
        {
            _singerAdminService = singerAdminService;
        }

        public string ActionName => "GetSingers";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            return await _singerAdminService.GetAllSingersAsync(cancellationToken);
        }
    }
}
