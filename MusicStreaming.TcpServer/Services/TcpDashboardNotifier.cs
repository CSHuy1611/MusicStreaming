using Microsoft.AspNetCore.SignalR.Client;
using MusicStreaming.Application.Common.Interfaces.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Services
{
    public class TcpDashboardNotifier : IDashboardNotifier
    {
        private readonly HubConnection _hubConnection;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);

        public TcpDashboardNotifier()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7242/dashboardHub", options =>
                {
                    options.HttpMessageHandlerFactory = handler => new System.Net.Http.HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
                    };
                    options.WebSocketConfiguration = sockets =>
                    {
                        sockets.RemoteCertificateValidationCallback = (sender, certificate, chain, policyErrors) => true;
                    };
                })
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                .Build();

            _hubConnection.Closed += error =>
            {
                Console.WriteLine($"[SignalR] Mất kết nối tới Web API. Lý do: {error?.Message ?? "không rõ"}");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnecting += error =>
            {
                Console.WriteLine($"[SignalR] Đang thử kết nối lại tới Web API...");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += connectionId =>
            {
                Console.WriteLine($"[SignalR] Đã kết nối lại thành công. ConnectionId: {connectionId}");
                return Task.CompletedTask;
            };
        }

        public async Task BroadcastDashboardUpdateAsync()
        {
            try
            {
                await EnsureConnectedAsync();

                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("TriggerUpdateFromTcp");
                    Console.WriteLine("[SignalR] Đã báo Dashboard cập nhật thành công.");
                }
                else
                {
                    Console.WriteLine($"[SignalR Warning]: Không thể gửi - State hiện tại: {_hubConnection.State}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SignalR Warning]: Broadcast thất bại. {ex.Message}");
            }
        }

        private async Task EnsureConnectedAsync()
        {

            if (_hubConnection.State != HubConnectionState.Disconnected)
                return;


            await _connectionLock.WaitAsync();
            try
            {
                if (_hubConnection.State == HubConnectionState.Disconnected)
                {
                    Console.WriteLine("[SignalR] Đang khởi tạo kết nối tới Web API...");
                    await _hubConnection.StartAsync();
                    Console.WriteLine("[SignalR] Kết nối thành công.");
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }
    }
}
