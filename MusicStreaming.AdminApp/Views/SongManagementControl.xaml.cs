using Microsoft.Win32;
using MusicStreaming.AdminApp.Interfaces;
using MusicStreaming.AdminApp.Models;
using MusicStreaming.AdminApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicStreaming.AdminApp.Views
{
    public partial class SongManagementControl : UserControl
    {
        private readonly TcpClientService _tcpClient;
        private readonly ICloudinaryService _cloudinaryService;
        private int _selectedSongId = 0;
        private byte[]? _selectedAudioBytes;
        private byte[]? _selectedImageBytes;
        private string _selectedAudioName = string.Empty;
        private string _selectedImageName = string.Empty;

        public SongManagementControl()
        {
            InitializeComponent();
            _tcpClient = new TcpClientService("127.0.0.1", 5000);
            _cloudinaryService = new CloudinaryService("dctej40r7", "331333545287216", "1z8nGUu3e-Bj8INVkMemY8xaq48");
            Loaded += SongManagementControl_Loaded;
        }

        private async void SongManagementControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadComboBoxDataAsync();
            await LoadSongsAsync();
        }

        private async Task LoadComboBoxDataAsync()
        {
            try
            {
                var singerResponse = await _tcpClient.SendRequestAsync("GetSingersSong", new { });
                if (singerResponse.IsSuccess)
                {
                    var singers = JsonSerializer.Deserialize<List<SingerDto>>(singerResponse.Data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    CboSinger.ItemsSource = singers;
                }
                else
                {
                    string errorMsg = !string.IsNullOrEmpty(singerResponse.Message) ? singerResponse.Message : singerResponse.Data;
                    MessageBox.Show($"Lỗi lấy danh sách Ca sĩ từ Server:\n{errorMsg}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                var genreResponse = await _tcpClient.SendRequestAsync("GetGenresSong", new { });
                if (genreResponse.IsSuccess)
                {
                    var genres = JsonSerializer.Deserialize<List<GenreDto>>(genreResponse.Data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    CboGenre.ItemsSource = genres;
                }
                else
                {
                    string errorMsg = !string.IsNullOrEmpty(genreResponse.Message) ? genreResponse.Message : genreResponse.Data;
                    MessageBox.Show($"Lỗi lấy danh sách Thể loại từ Server:\n{errorMsg}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ngoại lệ kết nối: {ex.Message}", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task LoadSongsAsync()
        {
            BtnRefresh.IsEnabled = false;
            try
            {
                var response = await _tcpClient.SendRequestAsync("GetSongs", new { });
                if (response.IsSuccess)
                {
                    var songs = JsonSerializer.Deserialize<List<SongAdminDto>>(response.Data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    SongsDataGrid.ItemsSource = songs;
                }
                else
                {
                    string errorMsg = !string.IsNullOrEmpty(response.Message) ? response.Message : response.Data;
                    MessageBox.Show($"Lỗi tải danh sách bài hát: {errorMsg}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            finally
            {
                BtnRefresh.IsEnabled = true;
            }
        }

        private void SongsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SongsDataGrid.SelectedItem is SongAdminDto selectedSong)
            {
                _selectedSongId = selectedSong.Id;
                TxtTitle.Text = selectedSong.Title;

                CboSinger.SelectedValue = selectedSong.SingerId;
                CboGenre.SelectedValue = selectedSong.GenreId;

                TxtDuration.Text = selectedSong.Duration.ToString();
                ChkVip.IsChecked = selectedSong.IsVip;

                ClearFileStateOnly();
                TxtAudioFile.Text = "(Giữ nguyên file cũ hoặc chọn file mới)";
                TxtImageFile.Text = "(Giữ nguyên file cũ hoặc chọn file mới)";
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedSongId = 0;
            TxtTitle.Clear();
            CboSinger.SelectedItem = null;
            CboGenre.SelectedItem = null;
            TxtDuration.Text = "00:03:00";
            ChkVip.IsChecked = false;
            SongsDataGrid.SelectedItem = null;

            ClearFileStateOnly();
            TxtAudioFile.Clear();
            TxtImageFile.Clear();
        }

        private void ClearFileStateOnly()
        {
            _selectedAudioBytes = null;
            _selectedImageBytes = null;
            _selectedAudioName = string.Empty;
            _selectedImageName = string.Empty;
        }

        private void BtnBrowseAudio_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Audio Files (*.mp3;*.wav)|*.mp3;*.wav",
                Title = "Chọn File Nhạc"
            };

            if (ofd.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(ofd.FileName);
                if (fileInfo.Length > 20 * 1024 * 1024)
                {
                    MessageBox.Show("File nhạc phải dưới 20MB", "Vượt quá dung lượng", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _selectedAudioBytes = File.ReadAllBytes(ofd.FileName);
                _selectedAudioName = fileInfo.Name;
                TxtAudioFile.Text = ofd.FileName;
            }
        }

        private void BtnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Chọn Ảnh Bìa"
            };

            if (ofd.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(ofd.FileName);
                if (fileInfo.Length > 5 * 1024 * 1024)
                {
                    MessageBox.Show("File ảnh phải dưới 5MB", "Vượt quá dung lượng", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _selectedImageBytes = File.ReadAllBytes(ofd.FileName);
                _selectedImageName = fileInfo.Name;
                TxtImageFile.Text = ofd.FileName;
            }
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadSongsAsync();
        }

        private bool ValidateInput(out int singerId, out int genreId, out TimeSpan duration)
        {
            singerId = 0;
            genreId = 0;
            duration = TimeSpan.Zero;

            if (string.IsNullOrWhiteSpace(TxtTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên bài hát.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (CboSinger.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Ca sĩ.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            singerId = (int)CboSinger.SelectedValue;

            if (CboGenre.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Thể loại.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            genreId = (int)CboGenre.SelectedValue;

            if (!TimeSpan.TryParse(TxtDuration.Text, out duration))
            {
                MessageBox.Show("Vui lòng nhập định dạng Thời lượng hợp lệ (hh:mm:ss).", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // --- CÁC HÀM HỖ TRỢ PROGRESS BAR ---
        private async Task SimulateProgressAsync(IProgress<int> progress, CancellationToken token)
        {
            int current = 0;
            var random = new Random();
            try
            {
                while (!token.IsCancellationRequested && current < 95)
                {
                    current += random.Next(5, 12); // File nhạc nặng nên cho nhảy % chậm hơn một chút
                    if (current > 95) current = 95;

                    progress.Report(current);
                    await Task.Delay(random.Next(300, 600), token);
                }
            }
            catch (TaskCanceledException)
            {
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
        // ------------------------------------

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAudioBytes == null || _selectedImageBytes == null)
            {
                MessageBox.Show("Vui lòng chọn cả file Nhạc và file Ảnh trước khi thêm.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateInput(out int singerId, out int genreId, out TimeSpan duration)) return;

            BtnAdd.IsEnabled = false;
            ShowProgressOverlay("Chuẩn bị dữ liệu...");

            using var cts = new CancellationTokenSource();

            try
            {
                // Bật giả lập phần trăm
                var progress = new Progress<int>(percent =>
                {
                    PbUpload.Value = percent;
                    TxtProgressInfo.Text = $"Đang tải file lên Cloud... {percent}%";
                });
                var progressTask = SimulateProgressAsync(progress, cts.Token);

                // Upload song song cả nhạc và ảnh
                var audioUploadTask = _cloudinaryService.UploadFileAsync(_selectedAudioBytes, _selectedAudioName, "songs/audio");
                var imageUploadTask = _cloudinaryService.UploadFileAsync(_selectedImageBytes, _selectedImageName, "songs/images");

                await Task.WhenAll(audioUploadTask, imageUploadTask);

                // Hoàn tất tải file
                cts.Cancel();
                PbUpload.Value = 100;
                TxtProgressInfo.Text = "Tải file hoàn tất 100%";
                await Task.Delay(300);

                string audioUrl = audioUploadTask.Result;
                string imageUrl = imageUploadTask.Result;

                // Chuyển sang xoay vòng chờ TCP Server
                PbUpload.IsIndeterminate = true;
                TxtProgressInfo.Text = "Đang lưu dữ liệu vào hệ thống...";

                var dto = new CreateSongDto
                {
                    Title = TxtTitle.Text,
                    Duration = duration,
                    IsVip = ChkVip.IsChecked ?? false,
                    SingerId = singerId,
                    GenreId = genreId,
                    AudioUrl = audioUrl,
                    AudioFileName = _selectedAudioName,
                    ImageUrl = imageUrl,
                    ImageFileName = _selectedImageName
                };

                var response = await _tcpClient.SendRequestAsync("CreateSong", dto);

                HideProgressOverlay();

                if (response.IsSuccess)
                {
                    MessageBox.Show("Đã thêm bài hát thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    await LoadSongsAsync();
                }
                else
                {
                    string errorMsg = !string.IsNullOrEmpty(response.Message) ? response.Message : response.Data;
                    MessageBox.Show(errorMsg, "Lỗi thêm mới", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                HideProgressOverlay();
                MessageBox.Show($"Có lỗi trong quá trình xử lý: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                cts.Cancel();
                HideProgressOverlay();
                BtnAdd.IsEnabled = true;
            }
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSongId == 0)
            {
                MessageBox.Show("Vui lòng chọn một bài hát để cập nhật.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateInput(out int singerId, out int genreId, out TimeSpan duration)) return;

            BtnUpdate.IsEnabled = false;
            ShowProgressOverlay("Chuẩn bị dữ liệu...");

            using var cts = new CancellationTokenSource();

            try
            {
                var uploadTasks = new List<Task>();
                Task<string> audioUploadTask = null;
                Task<string> imageUploadTask = null;

                bool hasFileUpload = _selectedAudioBytes != null || _selectedImageBytes != null;

                if (hasFileUpload)
                {
                    var progress = new Progress<int>(percent =>
                    {
                        PbUpload.Value = percent;
                        TxtProgressInfo.Text = $"Đang tải file lên Cloud... {percent}%";
                    });
                    var progressTask = SimulateProgressAsync(progress, cts.Token);

                    if (_selectedAudioBytes != null)
                    {
                        audioUploadTask = _cloudinaryService.UploadFileAsync(_selectedAudioBytes, _selectedAudioName, "songs/audio");
                        uploadTasks.Add(audioUploadTask);
                    }

                    if (_selectedImageBytes != null)
                    {
                        imageUploadTask = _cloudinaryService.UploadFileAsync(_selectedImageBytes, _selectedImageName, "songs/images");
                        uploadTasks.Add(imageUploadTask);
                    }

                    await Task.WhenAll(uploadTasks);

                    cts.Cancel();
                    PbUpload.Value = 100;
                    TxtProgressInfo.Text = "Tải file hoàn tất 100%";
                    await Task.Delay(300);
                }

                string audioUrl = audioUploadTask != null ? audioUploadTask.Result : null;
                string imageUrl = imageUploadTask != null ? imageUploadTask.Result : null;

                PbUpload.IsIndeterminate = true;
                TxtProgressInfo.Text = "Đang lưu dữ liệu cập nhật...";

                var dto = new UpdateSongDto
                {
                    Id = _selectedSongId,
                    Title = TxtTitle.Text,
                    Duration = duration,
                    IsVip = ChkVip.IsChecked ?? false,
                    SingerId = singerId,
                    GenreId = genreId,
                    AudioUrl = audioUrl,
                    AudioFileName = _selectedAudioName,
                    ImageUrl = imageUrl,
                    ImageFileName = _selectedImageName
                };

                var response = await _tcpClient.SendRequestAsync("UpdateSong", dto);

                HideProgressOverlay();

                if (response.IsSuccess)
                {
                    MessageBox.Show("Cập nhật bài hát thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    await LoadSongsAsync();
                }
                else
                {
                    string errorMsg = !string.IsNullOrEmpty(response.Message) ? response.Message : response.Data;
                    MessageBox.Show(errorMsg, "Lỗi cập nhật", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                HideProgressOverlay();
                MessageBox.Show($"Có lỗi trong quá trình xử lý: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (_selectedSongId == 0)
            {
                MessageBox.Show("Vui lòng chọn một bài hát để xóa.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa bài hát '{TxtTitle.Text}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                BtnDelete.IsEnabled = false;
                try
                {
                    var response = await _tcpClient.SendRequestAsync("DeleteSong", _selectedSongId);
                    if (response.IsSuccess)
                    {
                        MessageBox.Show("Đã xóa bài hát thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                        await LoadSongsAsync();
                    }
                    else
                    {
                        string errorMsg = !string.IsNullOrEmpty(response.Message) ? response.Message : response.Data;
                        MessageBox.Show(errorMsg, "Lỗi xóa", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                finally
                {
                    BtnDelete.IsEnabled = true;
                }
            }
        }

        private void TxtDuration_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}