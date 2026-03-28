using System;
using MusicStreaming.TcpServer.Networking.Interfaces;

namespace MusicStreaming.TcpServer.Networking.Interfaces
{
    public interface IConnectionManager
    {
        void AddConnection(Guid connectionId, IClientHandler clientHandler);
        void RemoveConnection(Guid connectionId);
        void CloseAll();
        IEnumerable<IClientHandler> GetStaleConnections(TimeSpan idleTimeout);
    }
}
