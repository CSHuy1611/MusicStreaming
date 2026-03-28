using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Songs
{
    public class GetSongsEndpoint : IEndpointHandler
    {
        private readonly ISongAdminService _songAdminService;

        public GetSongsEndpoint(ISongAdminService songAdminService)
        {
            _songAdminService = songAdminService;
        }

        public string ActionName => "GetSongs";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            var songs = await _songAdminService.GetAllSongsAsync(cancellationToken);
            return songs;
        }
    }
}
