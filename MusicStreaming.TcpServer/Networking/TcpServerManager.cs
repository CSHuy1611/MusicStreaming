using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicStreaming.TcpServer.Networking.Interfaces;
using MusicStreaming.TcpServer.Protocol.Serialization;
using MusicStreaming.TcpServer.Routing.Interfaces;

namespace MusicStreaming.TcpServer.Networking
{
    public class TcpServerManager : ITcpServer
    {
        private readonly ILogger<TcpServerManager> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConnectionManager _connectionManager;
        private readonly IServiceProvider _serviceProvider;
        private TcpListener? _tcpListener;
        private CancellationTokenSource? _cts;
        private readonly SemaphoreSlim _connectionLimit = new SemaphoreSlim(200, 200); 

        public TcpServerManager(
            ILogger<TcpServerManager> logger, 
            IConfiguration configuration,
            IConnectionManager connectionManager,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionManager = connectionManager;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            int port = _configuration.GetValue<int>("TcpServer:Port", 5000);
            _tcpListener = new TcpListener(IPAddress.Any, port);
            
            try 
            {
                _tcpListener.Start();
                _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                
                _logger.LogInformation("TCP Server started on port {Port}.", port);

                // Chạy song song với việc chấp nhận client để không block thread chính
                // nghĩa là khi server đã sẵn sàng, nó sẽ tiếp tục chấp nhận các client mới mà không bị gián đoạn
                _ = AcceptClientsAsync(_cts.Token);
            }
            catch (SocketException ex)
            {
                _logger.LogError(ex, "Failed to start TCP Server on port {Port}. The port might be in use by another application.", port);
                // System continues without TCP server functionality, preventing a full app crash
            }

            return Task.CompletedTask;
        }

        private async Task AcceptClientsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                bool lockTaken = false;
                try
                {
                    await _connectionLimit.WaitAsync(token);
                    lockTaken = true;

                    var tcpClient = await _tcpListener!.AcceptTcpClientAsync(token);
                    _logger.LogInformation("New client connecting from {Endpoint}", tcpClient.Client.RemoteEndPoint);

                    // Dùng để đảm bảo mỗi client có một scope riêng biệt để quản lý lifetime của các service như IPacketSerializer, IRequestRouter, etc.
                    var scope = _serviceProvider.CreateScope();
                    var packetSerializer = scope.ServiceProvider.GetRequiredService<IPacketSerializer>();
                    var requestRouter = scope.ServiceProvider.GetRequiredService<IRequestRouter>();
                    var connectionLogger = scope.ServiceProvider.GetRequiredService<ILogger<ClientConnection>>();

                    var clientConnection = new ClientConnection(tcpClient, packetSerializer, requestRouter, connectionLogger);
                    
                    _connectionManager.AddConnection(clientConnection.ConnectionId, clientConnection);

                    // Dùng để xử lý client trong một task riêng biệt để không block việc chấp nhận các client khác
                    _ = Task.Run(async () => 
                    {
                        try
                        {
                            await clientConnection.ProcessAsync();
                        }
                        finally
                        {
                            _connectionManager.RemoveConnection(clientConnection.ConnectionId);
                            scope.Dispose(); // dùng để giải phóng scope và các service liên quan khi client ngắt kết nối
                            _connectionLimit.Release(); // Giải phóng slot kết nối
                        }
                    }, CancellationToken.None);
                }
                catch (OperationCanceledException)
                {
                    // dùng để xử lý khi token bị hủy, thường là khi server đang dừng lại
                    if (lockTaken) _connectionLimit.Release();
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error accepting TCP client.");
                    if (lockTaken) _connectionLimit.Release();
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping TCP Server. Closing all connections...");
            
            _cts?.Cancel();
            _tcpListener?.Stop();
            _connectionManager.CloseAll();
            
            _logger.LogInformation("TCP Server stopped.");

            return Task.CompletedTask;
        }
    }
}
