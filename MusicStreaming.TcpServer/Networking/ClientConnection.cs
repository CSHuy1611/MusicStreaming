using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MusicStreaming.TcpServer.Networking.Interfaces;
using MusicStreaming.TcpServer.Protocol.Models;
using MusicStreaming.TcpServer.Protocol.Serialization;
using MusicStreaming.TcpServer.Routing.Interfaces;

namespace MusicStreaming.TcpServer.Networking
{
    public class ClientConnection : IClientHandler
    {
        private readonly TcpClient _tcpClient;
        private readonly IPacketSerializer _packetSerializer;
        private readonly IRequestRouter _requestRouter;
        private readonly ILogger<ClientConnection> _logger;
        private readonly NetworkStream _stream;
        private readonly CancellationTokenSource _cts = new();
        private bool _isDisconnected;
        
        public Guid ConnectionId { get; } = Guid.NewGuid();
        public DateTime LastActivity { get; private set; } = DateTime.UtcNow;

        public ClientConnection(
            TcpClient tcpClient, 
            IPacketSerializer packetSerializer, 
            IRequestRouter requestRouter,
            ILogger<ClientConnection> logger)
        {
            _tcpClient = tcpClient;
            _packetSerializer = packetSerializer;
            _requestRouter = requestRouter;
            _logger = logger;
            _stream = _tcpClient.GetStream();
        }

        public async Task ProcessAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", ConnectionId);
            
            try
            {
                var lengthBuffer = new byte[4];

                while (!_cts.Token.IsCancellationRequested && _tcpClient.Connected)
                {
                    // Update activity timer
                    LastActivity = DateTime.UtcNow;

                    // Read length prefix (4 bytes)
                    int bytesRead = await ReadExactAsync(lengthBuffer, 4, _cts.Token);
                    if (bytesRead == 0) break; // Client gracefully closed

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(lengthBuffer);
                    }

                    int expectedLength = BitConverter.ToInt32(lengthBuffer, 0);
                    if (expectedLength <= 0 || expectedLength > 30 * 1024 * 1024) // 30 MB limit
                    {
                        _logger.LogWarning("Invalid payload length {Length} received from {ConnectionId}. Disconnecting.", expectedLength, ConnectionId);
                        break;
                    }

                    // Read payload
                    var payloadBuffer = new byte[expectedLength];
                    bytesRead = await ReadExactAsync(payloadBuffer, expectedLength, _cts.Token);
                    if (bytesRead == 0) break;

                    LastActivity = DateTime.UtcNow;

                    // Deserialize
                    var request = _packetSerializer.Deserialize<TcpRequest>(payloadBuffer);
                    if (request == null)
                    {
                        _logger.LogWarning("Failed to deserialize packet from {ConnectionId}. Disconnecting.", ConnectionId);
                        break;
                    }

                    _logger.LogInformation("Request received from {ConnectionId}: {Action} [ReqId: {RequestId}]", ConnectionId, request.Action, request.RequestId);

                    // Route request
                    var response = await _requestRouter.RouteAsync(request, _cts.Token);

                    // Serialize response
                    var responseData = _packetSerializer.Serialize(response);
                    
                    // Write length prefix followed by response data
                    var responseLengthPrefix = BitConverter.GetBytes(responseData.Length);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(responseLengthPrefix);
                    }
                    await _stream.WriteAsync(responseLengthPrefix, 0, 4, _cts.Token);
                    await _stream.WriteAsync(responseData, 0, responseData.Length, _cts.Token);
                    
                    LastActivity = DateTime.UtcNow;
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Connection {ConnectionId} was canceled.", ConnectionId);
            }
            catch (IOException ex)
            {
                _logger.LogWarning("IO Error on connection {ConnectionId}: {Message}", ConnectionId, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred processing connection {ConnectionId}", ConnectionId);
            }
            finally
            {
                Disconnect();
            }
        }

        private async Task<int> ReadExactAsync(byte[] buffer, int count, CancellationToken cancellationToken)
        {
            int offset = 0;
            // Provide a read timeout to avoid hanging forever on partial packets
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(60));

            while (offset < count)
            {
                int read = await _stream.ReadAsync(buffer, offset, count - offset, timeoutCts.Token);
                if (read == 0) return 0; // Stream closed
                offset += read;
            }
            return offset;
        }

        public void Disconnect()
        {
            if (_isDisconnected) return;
            _isDisconnected = true;
            
            _logger.LogInformation("Client disconnected: {ConnectionId}", ConnectionId);
            
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
            
            _tcpClient.Close();
            _tcpClient.Dispose();
            _cts.Dispose();
        }
    }
}
