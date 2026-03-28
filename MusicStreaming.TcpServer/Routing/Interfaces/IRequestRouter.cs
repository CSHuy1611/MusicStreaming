using System.Threading;
using System.Threading.Tasks;
using MusicStreaming.TcpServer.Protocol.Models;

namespace MusicStreaming.TcpServer.Routing.Interfaces
{
    public interface IRequestRouter
    {
        Task<TcpResponse> RouteAsync(TcpRequest request, CancellationToken cancellationToken);
    }
}
