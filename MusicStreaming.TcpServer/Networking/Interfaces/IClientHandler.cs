using System;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Networking.Interfaces
{
    public interface IClientHandler
    {
        Guid ConnectionId { get; }
        DateTime LastActivity { get; }
        Task ProcessAsync();
        void Disconnect();
    }
}
