using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using MusicStreaming.AdminApp.Models;
using MusicStreaming.AdminApp.Services;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MusicStreaming.AdminApp.Views
{
    public partial class DashboardControl : UserControl
    {
        private readonly TcpClientService _tcpClient;
        private HubConnection? _hubConnection;

        public DashboardControl()
        {
            InitializeComponent();
            _tcpClient = new TcpClientService("127.0.0.1", 5000);
            Loaded += DashboardControl_Loaded;
            Unloaded += DashboardControl_Unloaded; // Cần ngắt kết nối khi chuyển tab
        }

        private async void DashboardControl_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. Load data ban đầu qua TCP
            await LoadInitialDataAsync();

            // 2. Thiết lập và khởi chạy SignalR
            await SetupSignalRAsync();
        }

        private async void DashboardControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }

        private async Task LoadInitialDataAsync()
        {
            try
            {
                var response = await _tcpClient.SendRequestAsync("GetDashboardData", new { });
                if (response.IsSuccess)
                {
                    var data = JsonSerializer.Deserialize<DashboardDataDto>(response.Data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (data != null) UpdateUI(data);
                }
                else
                {
                    MessageBox.Show("Lỗi lấy dữ liệu Dashboard: " + response.Data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối TCP: " + ex.Message);
            }
        }

        private async Task SetupSignalRAsync()
        {
            string hubUrl = "http://localhost:5244/dashboardHub";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
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
                .WithAutomaticReconnect()
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
                })
                .Build();

            // Lắng nghe sự kiện "ReceiveDashboardUpdate" từ Server bắn về
            _hubConnection.On<DashboardDataDto>("ReceiveDashboardUpdate", (data) =>
            {
                // SignalR chạy trên luồng phụ, phải dùng Dispatcher để cập nhật UI chính
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateUI(data);
                });
            });

            _hubConnection.Closed += async (error) =>
            {
                Application.Current.Dispatcher.Invoke(() => UpdateSignalRStatus(false, "Mất kết nối. Đang thử lại..."));
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _hubConnection.StartAsync();
            };

            try
            {
                await _hubConnection.StartAsync();
                UpdateSignalRStatus(true, "Đã kết nối Real-time (SignalR)");
            }
            catch (Exception ex)
            {
                UpdateSignalRStatus(false, "Lỗi kết nối SignalR: " + ex.Message);
            }
        }

        private void UpdateSignalRStatus(bool isConnected, string message)
        {
            TxtSignalRStatus.Text = message;
            SignalRStatusDot.Fill = isConnected ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.Red);
        }

        // Hàm đổ dữ liệu lên các thẻ (Cards) và bảng
        private void UpdateUI(DashboardDataDto data)
        {
            // Update User Stats
            TxtTotalUsers.Text = data.UserStats.TotalUsers.ToString("N0");
            TxtVipNormalUsers.Text = $"{data.UserStats.VipUsers} VIP - {data.UserStats.NormalUsers} Thường";

            // Update Song Stats
            TxtTotalSongs.Text = data.SongStats.TotalSongs.ToString("N0");
            TxtActiveDisabledSongs.Text = $"{data.SongStats.ActiveSongs} Đang phát - {data.SongStats.DisabledSongs} Đã ẩn";

            // Update Revenue
            TxtMonthRevenue.Text = data.RevenueStats.CurrentMonthRevenue.ToString("N0") + " ₫";
            TxtYearRevenue.Text = data.RevenueStats.CurrentYearRevenue.ToString("N0") + " ₫";

            // Bảng xếp hạng
            GridTopListened.ItemsSource = data.TopListenedSongs;
            GridTopLiked.ItemsSource = data.TopLikedSongs;
        }

    }
}