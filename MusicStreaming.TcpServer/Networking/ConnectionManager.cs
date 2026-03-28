using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MusicStreaming.TcpServer.Networking.Interfaces;

namespace MusicStreaming.TcpServer.Networking
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<Guid, IClientHandler> _connections = new();
        private readonly ILogger<ConnectionManager> _logger;

        public ConnectionManager(ILogger<ConnectionManager> logger)
        {
            _logger = logger;
        }

        public void AddConnection(Guid connectionId, IClientHandler clientHandler)
        {
            if (_connections.TryAdd(connectionId, clientHandler))
            {
                _logger.LogInformation("Connection {ConnectionId} added to manager. Total connections: {Count}", connectionId, _connections.Count);
            }
        }

        public void RemoveConnection(Guid connectionId)
        {
            if (_connections.TryRemove(connectionId, out _))
            {
                _logger.LogInformation("Connection {ConnectionId} removed from manager. Total connections: {Count}", connectionId, _connections.Count);
            }
        }

        public IEnumerable<IClientHandler> GetStaleConnections(TimeSpan idleTimeout)
        {
            var threshold = DateTime.UtcNow - idleTimeout;
            return _connections.Values.Where(c => c.LastActivity < threshold).ToList();
        }

        public void CloseAll()
        {
            _logger.LogInformation("Closing all {Count} active connections...", _connections.Count);
            foreach (var connection in _connections.Values)
            {
                try
                {
                    connection.Disconnect();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while forcefully closing connection {ConnectionId}", connection.ConnectionId);
                }
            }
            _connections.Clear();
            _logger.LogInformation("All connections closed.");
        }
    }
}
