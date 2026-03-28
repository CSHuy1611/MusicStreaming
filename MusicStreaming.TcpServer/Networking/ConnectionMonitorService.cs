using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MusicStreaming.TcpServer.Networking.Interfaces;

namespace MusicStreaming.TcpServer.Networking
{
    public class ConnectionMonitorService : BackgroundService
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<ConnectionMonitorService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10);
        private readonly TimeSpan _idleTimeout = TimeSpan.FromSeconds(60);

        public ConnectionMonitorService(IConnectionManager connectionManager, ILogger<ConnectionMonitorService> logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ConnectionMonitorService started. Checking every {Interval}s for connections idle for more than {Timeout}s.", _checkInterval.TotalSeconds, _idleTimeout.TotalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var staleConnections = _connectionManager.GetStaleConnections(_idleTimeout);
                    foreach (var connection in staleConnections)
                    {
                        _logger.LogWarning("Connection {ConnectionId} exceeded idle timeout. Forcibly disconnecting.", connection.ConnectionId);
                        connection.Disconnect();
                        _connectionManager.RemoveConnection(connection.ConnectionId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking for stale connections.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("ConnectionMonitorService stopping.");
        }
    }
}
