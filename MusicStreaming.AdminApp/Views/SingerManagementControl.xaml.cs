using Microsoft.Win32;
using MusicStreaming.AdminApp.Interfaces;
using MusicStreaming.AdminApp.Models;
using MusicStreaming.AdminApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicStreaming.AdminApp.Views
{
    public partial class SingerManagementControl : UserControl
    {
        private readonly TcpClientService _tcpClient;
        private readonly ICloudinaryService _cloudinaryService;
        private int _selectedSingerId = 0;
        private byte[]? _selectedAvatarBytes;
        private string _selectedAvatarName = string.Empty;

        public SingerManagementControl()
        {
            InitializeComponent();
            _tcpClient = new TcpClientService("127.0.0.1", 5000);

            _cloudinaryService = new CloudinaryService("dctej40r7", "331333545287216", "1z8nGUu3e-Bj8INVkMemY8xaq48");

            Loaded += SingerManagementControl_Loaded;
        }

        private async void SingerManagementControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadSingersAsync();
        }

        private async Task LoadSingersAsync()
        {
            BtnRefresh.IsEnabled = false;
            try
            {
                var response = await _tcpClient.SendRequestAsync("GetSingers", new { });
                if (response.IsSuccess)
                {
                    var singers = JsonSerializer.Deserialize<List<SingerAdminDto>>(response.Data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    SingersDataGrid.ItemsSource = singers;
                }
                else
                {
                    MessageBox.Show($"Failed to load singers: {response.Data}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                BtnRefresh.IsEnabled = true;
            }
        }

        private void SingersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SingersDataGrid.SelectedItem is SingerAdminDto selectedSinger)
            {
                _selectedSingerId = selectedSinger.Id;
                TxtName.Text = selectedSinger.Name;
                TxtBio.Text = selectedSinger.Bio;

                ClearFileStateOnly();
                TxtAvatarFile.Text = "(Giữ nguyên ảnh cũ hoặc chọn ảnh mới)";
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedSingerId = 0;
            TxtName.Clear();
            TxtBio.Clear();
            SingersDataGrid.SelectedItem = null;

            ClearFileStateOnly();
            TxtAvatarFile.Clear();
        }

        private void ClearFileStateOnly()
        {
            _selectedAvatarBytes = null;
            _selectedAvatarName = string.Empty;
        }

        private void BtnBrowseAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Select Avatar File"
            };

            if (ofd.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(ofd.FileName);
                if (fileInfo.Length > 5 * 1024 * 1024) // Giới hạn 5MB
                {
                    MessageBox.Show("Dung lượng file nhỏ hơn 5MB", "Limit Exceeded", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _selectedAvatarBytes = File.ReadAllBytes(ofd.FileName);
                _selectedAvatarName = fileInfo.Name;
                TxtAvatarFile.Text = ofd.FileName;
            }
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadSingersAsync();
        }

        private async Task SimulateProgressAsync(IProgress<int> progress, CancellationToken token)
        {
            int current = 0;
            var random = new Random();
            try
            {
                while (!token.IsCancellationRequested && current < 95)
                {
                    current += random.Next(5, 15);
                    if (current > 95) current = 95;

                    progress.Report(current);
                    await Task.Delay(random.Next(200, 400), token);
                }
            }
            catch (TaskCanceledException)
            {
                // Bỏ qua lỗi khi token báo cancel
            }
        }

        private void ShowProgressOverlay(string initialText)
        {
            ProgressOverlay.Visibility = Visibility.Visible;
            PbUpload.IsIndeterminate = false;
            PbUpload.Value = 0;
            TxtProgressInfo.Text = initialText;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        private void HideProgressOverlay()
        {
            ProgressOverlay.Visibility = Visibility.Collapsed;
            Mouse.OverrideCursor = null;
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            BtnAdd.IsEnabled = false;
            ShowProgressOverlay("Chuẩn bị dữ liệu...");

            using var cts = new CancellationTokenSource();

            try
            {
                string avatarUrl = string.Empty;

                if (_selectedAvatarBytes != null && _selectedAvatarBytes.Length > 0)
                {
                    var progress = new Progress<int>(percent =>
                    {
                        PbUpload.Value = percent;
                        TxtProgressInfo.Text = $"Đang tải ảnh lên Cloud... {percent}%";
                    });

                    var progressTask = SimulateProgressAsync(progress, cts.Token);

                    avatarUrl = await _cloudinaryService.UploadFileAsync(_selectedAvatarBytes, _selectedAvatarName, "Singers/Avatars");

                    cts.Cancel();
                    PbUpload.Value = 100;
                    TxtProgressInfo.Text = "Tải ảnh hoàn tất 100%";
                    await Task.Delay(300);
                }

                // Chuyển thanh tiến độ sang dạng chạy vô định (xoay vòng) khi đợi DB
                PbUpload.IsIndeterminate = true;
                TxtProgressInfo.Text = "Đang lưu dữ liệu vào hệ thống...";

                var dto = new CreateSingerDto
                {
                    Name = TxtName.Text,
                    Bio = TxtBio.Text,
                    AvatarUrl = avatarUrl,
                    AvatarFileName = _selectedAvatarName
                };

                var response = await _tcpClient.SendRequestAsync("CreateSinger", dto);

                // Ẩn bảng Loading trước khi hiện thông báo
                HideProgressOverlay();

                if (response.IsSuccess)
                {
                    MessageBox.Show("Thêm Ca sĩ mới thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    await LoadSingersAsync();
                }
                else
                {
                    MessageBox.Show($"Lỗi khi thêm mới:\n{response.Data}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                HideProgressOverlay();
                MessageBox.Show($"Lỗi quá trình xử lý: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                cts.Cancel();
                HideProgressOverlay(); // Luôn đảm bảo Overlay tắt đi
                BtnAdd.IsEnabled = true;
            }
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSingerId == 0)
            {
                MessageBox.Show("Vui lòng chọn Ca sĩ để cập nhật.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Tên Ca sĩ không được trống.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            BtnUpdate.IsEnabled = false;
            ShowProgressOverlay("Chuẩn bị dữ liệu...");

            using var cts = new CancellationTokenSource();

            try
            {
                string avatarUrl = string.Empty;

                if (_selectedAvatarBytes != null && _selectedAvatarBytes.Length > 0)
                {
                    var progress = new Progress<int>(percent =>
                    {
                        PbUpload.Value = percent;
                        TxtProgressInfo.Text = $"Đang tải ảnh lên Cloud... {percent}%";
                    });

                    var progressTask = SimulateProgressAsync(progress, cts.Token);

                    avatarUrl = await _cloudinaryService.UploadFileAsync(_selectedAvatarBytes, _selectedAvatarName, "Singers/Avatars");

                    cts.Cancel();
                    PbUpload.Value = 100;
                    TxtProgressInfo.Text = "Tải ảnh hoàn tất 100%";
                    await Task.Delay(300);
                }

                PbUpload.IsIndeterminate = true;
                TxtProgressInfo.Text = "Đang lưu dữ liệu cập nhật...";

                var dto = new UpdateSingerDto
                {
                    Id = _selectedSingerId,
                    Name = TxtName.Text,
                    Bio = TxtBio.Text,
                    AvatarUrl = avatarUrl,
                    AvatarFileName = _selectedAvatarName
                };

                var response = await _tcpClient.SendRequestAsync("UpdateSinger", dto);

                HideProgressOverlay();

                if (response.IsSuccess)
                {
                    MessageBox.Show("Cập nhật ca sĩ thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    await LoadSingersAsync();
                }
                else
                {
                    MessageBox.Show($"Lỗi khi cập nhật:\n{response.Data}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                HideProgressOverlay();
                MessageBox.Show($"Lỗi quá trình xử lý: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                cts.Cancel();
                HideProgressOverlay();
                BtnUpdate.IsEnabled = true;
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSingerId == 0)
            {
                MessageBox.Show("Vui lòng chọn ca sĩ để xóa.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn xóa ca sĩ '{TxtName.Text}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                BtnDelete.IsEnabled = false;
                try
                {
                    var response = await _tcpClient.SendRequestAsync("DeleteSinger", _selectedSingerId);
                    if (response.IsSuccess)
                    {
                        MessageBox.Show("Đã xóa cá sĩ thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                        await LoadSingersAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Lỗi khi xóa ca sĩ:\n{response.Data}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                finally
                {
                    BtnDelete.IsEnabled = true;
                }
            }
        }
    }
}