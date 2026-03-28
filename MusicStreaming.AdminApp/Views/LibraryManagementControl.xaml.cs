using MusicStreaming.AdminApp.Models;
using MusicStreaming.AdminApp.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicStreaming.AdminApp.Views
{
    public partial class LibraryManagementControl : UserControl
    {
        private readonly TcpClientService _tcpClient;

        private int _selectedGenreId = 0;
        private int _selectedPlaylistId = 0;
        private int _selectedSongInPlaylistId = 0;

        public LibraryManagementControl()
        {
            InitializeComponent();
            _tcpClient = new TcpClientService("127.0.0.1", 5000);
            Loaded += LibraryManagementControl_Loaded;
        }

        private async void LibraryManagementControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadGenresAsync();
            await LoadSystemPlaylistsAsync();
            await LoadAllSongsForComboBoxAsync(); 
        }

        private void ShowError(TcpResponse response, string title = "Lỗi")
        {
            string msg = !string.IsNullOrEmpty(response.Message) ? response.Message : response.Data;
            MessageBox.Show(msg, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SetLoadingState(Button btn, bool isLoading, string loadingText = "Đang xử lý...", string normalText = "")
        {
            if (isLoading)
            {
                btn.IsEnabled = false;
                btn.Content = loadingText;
                Mouse.OverrideCursor = Cursors.Wait;
            }
            else
            {
                btn.IsEnabled = true;
                btn.Content = normalText;
                Mouse.OverrideCursor = null;
            }
        }

        // ==========================================
        // VÙNG XỬ LÝ: THỂ LOẠI (GENRE)
        // ==========================================
        private async Task LoadGenresAsync()
        {
            var response = await _tcpClient.SendRequestAsync("GetGenresLib", new { }); 
            if (response.IsSuccess)
            {
                var genres = JsonSerializer.Deserialize<List<GenreAdminDto>>(response.Data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                GenresDataGrid.ItemsSource = genres;
            }
            else ShowError(response, "Lỗi tải Thể loại");
        }

        private void GenresDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GenresDataGrid.SelectedItem is GenreAdminDto selected)
            {
                _selectedGenreId = selected.Id;
                TxtGenreName.Text = selected.Name;
            }
        }

        private void BtnClearGenre_Click(object sender, RoutedEventArgs e)
        {
            _selectedGenreId = 0;
            TxtGenreName.Clear();
            GenresDataGrid.SelectedItem = null;
        }

        private async void BtnRefreshGenre_Click(object sender, RoutedEventArgs e)
        {
            await LoadGenresAsync();
        }

        private async void BtnAddGenre_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtGenreName.Text)) return;

            SetLoadingState(BtnAddGenre, true, "Adding...", "Add");
            var response = await _tcpClient.SendRequestAsync("CreateGenreLib", new CreateGenreDto { Name = TxtGenreName.Text });
            SetLoadingState(BtnAddGenre, false, "", "Add");

            if (response.IsSuccess)
            {
                MessageBox.Show("Thêm Thể loại thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnClearGenre_Click(null, null);
                await LoadGenresAsync();
            }
            else ShowError(response);
        }

        private async void BtnUpdateGenre_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGenreId == 0 || string.IsNullOrWhiteSpace(TxtGenreName.Text)) return;

            SetLoadingState(BtnUpdateGenre, true, "Updating...", "Update");
            var response = await _tcpClient.SendRequestAsync("UpdateGenreLib", new UpdateGenreDto { Id = _selectedGenreId, Name = TxtGenreName.Text });
            SetLoadingState(BtnUpdateGenre, false, "", "Update");

            if (response.IsSuccess)
            {
                MessageBox.Show("Cập nhật Thể loại thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnClearGenre_Click(null, null);
                await LoadGenresAsync();
            }
            else ShowError(response);
        }

        private async void BtnDeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGenreId == 0) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa thể loại '{TxtGenreName.Text}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SetLoadingState(BtnDeleteGenre, true, "Deleting...", "Delete");
                var response = await _tcpClient.SendRequestAsync("DeleteGenreLib", _selectedGenreId);
                SetLoadingState(BtnDeleteGenre, false, "", "Delete");

                if (response.IsSuccess)
                {
                    MessageBox.Show("Xóa Thể loại thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    BtnClearGenre_Click(null, null);
                    await LoadGenresAsync();
                }
                else ShowError(response);
            }
        }

        // ==========================================
        // VÙNG XỬ LÝ: SYSTEM PLAYLIST
        // ==========================================
        private async Task LoadSystemPlaylistsAsync()
        {
            var response = await _tcpClient.SendRequestAsync("GetSystemPlaylists", new { });
            if (response.IsSuccess)
            {
                var playlists = JsonSerializer.Deserialize<List<PlaylistAdminDto>>(response.Data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                PlaylistsDataGrid.ItemsSource = playlists;
            }
            else ShowError(response, "Lỗi tải Playlists");
        }

        private async void PlaylistsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistsDataGrid.SelectedItem is PlaylistAdminDto selected)
            {
                _selectedPlaylistId = selected.Id;
                TxtPlaylistName.Text = selected.Name;

                TxtSelectedPlaylistTitle.Text = $"Đang cấu hình: {selected.Name}";
                PanelAddSongToPlaylist.IsEnabled = true;

                await LoadSongsInSelectedPlaylistAsync();
            }
            else
            {
                _selectedPlaylistId = 0;
                TxtSelectedPlaylistTitle.Text = "Vui lòng chọn 1 Playlist bên trái để cấu hình nhạc";
                PanelAddSongToPlaylist.IsEnabled = false;
                PlaylistSongsDataGrid.ItemsSource = null;
            }
        }

        private void BtnClearPlaylist_Click(object sender, RoutedEventArgs e)
        {
            _selectedPlaylistId = 0;
            TxtPlaylistName.Clear();
            PlaylistsDataGrid.SelectedItem = null;
        }

        private async void BtnRefreshPlaylist_Click(object sender, RoutedEventArgs e)
        {
            await LoadSystemPlaylistsAsync();
        }

        private async void BtnAddPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtPlaylistName.Text)) return;

            SetLoadingState(BtnAddPlaylist, true, "Adding...", "Add");
            var response = await _tcpClient.SendRequestAsync("CreateSystemPlaylist", new CreateSystemPlaylistDto { Name = TxtPlaylistName.Text });
            SetLoadingState(BtnAddPlaylist, false, "", "Add");

            if (response.IsSuccess)
            {
                MessageBox.Show("Thêm Playlist thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnClearPlaylist_Click(null, null);
                await LoadSystemPlaylistsAsync();
            }
            else ShowError(response);
        }

        private async void BtnUpdatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPlaylistId == 0 || string.IsNullOrWhiteSpace(TxtPlaylistName.Text)) return;

            SetLoadingState(BtnUpdatePlaylist, true, "Updating...", "Update");
            var response = await _tcpClient.SendRequestAsync("UpdateSystemPlaylist", new UpdateSystemPlaylistDto { Id = _selectedPlaylistId, Name = TxtPlaylistName.Text });
            SetLoadingState(BtnUpdatePlaylist, false, "", "Update");

            if (response.IsSuccess)
            {
                MessageBox.Show("Cập nhật Playlist thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnClearPlaylist_Click(null, null);
                await LoadSystemPlaylistsAsync();
            }
            else ShowError(response);
        }

        private async void BtnDeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPlaylistId == 0) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa playlist '{TxtPlaylistName.Text}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SetLoadingState(BtnDeletePlaylist, true, "Deleting...", "Delete");
                var response = await _tcpClient.SendRequestAsync("DeleteSystemPlaylist", _selectedPlaylistId);
                SetLoadingState(BtnDeletePlaylist, false, "", "Delete");

                if (response.IsSuccess)
                {
                    MessageBox.Show("Xóa Playlist thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    BtnClearPlaylist_Click(null, null);
                    await LoadSystemPlaylistsAsync();
                }
                else ShowError(response);
            }
        }

        // ==========================================
        // VÙNG XỬ LÝ: CHI TIẾT BÀI HÁT TRONG PLAYLIST
        // ==========================================


        private async Task LoadAllSongsForComboBoxAsync()
        {
            var response = await _tcpClient.SendRequestAsync("GetSongs", new { }); 
            if (response.IsSuccess)
            {
                var songs = JsonSerializer.Deserialize<List<SongAdminDto>>(response.Data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                CboAvailableSongs.ItemsSource = songs;
            }
        }

        private async Task LoadSongsInSelectedPlaylistAsync()
        {
            if (_selectedPlaylistId == 0) return;
            var response = await _tcpClient.SendRequestAsync("GetSongsInPlaylist", _selectedPlaylistId);
            if (response.IsSuccess)
            {
                var songs = JsonSerializer.Deserialize<List<PlaylistSongDto>>(response.Data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                PlaylistSongsDataGrid.ItemsSource = songs;
            }
            else ShowError(response, "Lỗi tải chi tiết Playlist");
        }

        private async void BtnAddSongToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPlaylistId == 0) return;
            if (CboAvailableSongs.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn 1 bài hát để thêm!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int songId = (int)CboAvailableSongs.SelectedValue;
            var dto = new AddSongToPlaylistDto { PlaylistId = _selectedPlaylistId, SongId = songId };

            SetLoadingState(BtnAddSongToPlaylist, true, "Đang xử lý...", "Thêm vào Playlist");
            var response = await _tcpClient.SendRequestAsync("AddSongToPlaylist", dto);
            SetLoadingState(BtnAddSongToPlaylist, false, "", "Thêm vào Playlist");

            if (response.IsSuccess)
            {
                await LoadSongsInSelectedPlaylistAsync(); 
                await LoadSystemPlaylistsAsync(); 
            }
            else ShowError(response);
        }

        private void PlaylistSongsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistSongsDataGrid.SelectedItem is PlaylistSongDto selected)
            {
                _selectedSongInPlaylistId = selected.SongId;
            }
            else
            {
                _selectedSongInPlaylistId = 0;
            }
        }


        private async void BtnRemoveSongFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPlaylistId == 0 || _selectedSongInPlaylistId == 0)
            {
                MessageBox.Show("Vui lòng chọn 1 bài hát trong bảng để xóa!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Xóa bài hát này khỏi Playlist?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var dto = new RemoveSongFromPlaylistDto { PlaylistId = _selectedPlaylistId, SongId = _selectedSongInPlaylistId };
                var response = await _tcpClient.SendRequestAsync("RemoveSongFromPlaylist", dto);

                if (response.IsSuccess)
                {
                    await LoadSongsInSelectedPlaylistAsync(); 
                    await LoadSystemPlaylistsAsync(); 
                }
                else ShowError(response);
            }
        }
    }
}