using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Interfaces
{
    public interface IEndpointHandler
    {
        string ActionName { get; }
        Task<object> HandleAsync(string payload, CancellationToken cancellationToken);
    }
}
