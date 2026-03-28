using MusicStreaming.AdminApp.Models;
using MusicStreaming.AdminApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicStreaming.AdminApp.Views
{
    public partial class SubscriptionManagementControl : UserControl
    {
        private readonly TcpClientService _tcpClient;
        private int _selectedPackageId = 0;

        public SubscriptionManagementControl()
        {
            InitializeComponent();
            _tcpClient = new TcpClientService("127.0.0.1", 5000);
            Loaded += SubscriptionManagementControl_Loaded;
        }

        private async void SubscriptionManagementControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPackagesAsync();
        }

        private async Task LoadPackagesAsync()
        {
            BtnRefresh.IsEnabled = false;
            try
            {
                var response = await _tcpClient.SendRequestAsync("GetSubscriptions", new { });
                if (response.IsSuccess)
                {
                    var packages = JsonSerializer.Deserialize<List<SubscriptionAdminDto>>(response.Data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    PackagesDataGrid.ItemsSource = packages;
                }
                else
                {
                    string errorMsg = !string.IsNullOrEmpty(response.Message) ? response.Message : response.Data;
                    MessageBox.Show($"Lỗi tải danh sách Gói VIP: {errorMsg}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ngoại lệ kết nối: {ex.Message}", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                BtnRefresh.IsEnabled = true;
            }
        }

        private void PackagesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PackagesDataGrid.SelectedItem is SubscriptionAdminDto selectedPackage)
            {
                _selectedPackageId = selectedPackage.Id;
                TxtName.Text = selectedPackage.Name;

 
                TxtPrice.Text = selectedPackage.Price.ToString("0");
                TxtDuration.Text = selectedPackage.DurationInMonths.ToString();
                ChkIsActive.IsChecked = selectedPackage.IsActive;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedPackageId = 0;
            TxtName.Clear();
            TxtPrice.Clear();
            TxtDuration.Clear();
            ChkIsActive.IsChecked = true; 
            PackagesDataGrid.SelectedItem = null;
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadPackagesAsync();
        }

        private bool ValidateInput(out decimal price, out int duration)
        {
            price = 0;
            duration = 0;

            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên gói VIP.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TxtPrice.Text, out price) || price < 0)
            {
                MessageBox.Show("Giá gói VIP phải là số và không được âm.", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TxtDuration.Text, out duration) || duration <= 0)
            {
                MessageBox.Show("Thời hạn gói (tháng) phải là số nguyên và lớn hơn 0.", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void ShowProgressOverlay(string initialText)
        {
            ProgressOverlay.Visibility = Visibility.Visible;
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
            if (!ValidateInput(out decimal price, out int duration)) return;

            BtnAdd.IsEnabled = false;
            ShowProgressOverlay("Đang lưu dữ liệu Gói VIP vào hệ thống...");

            try
            {
                var dto = new CreateSubscriptionDto
                {
                    Name = TxtName.Text,
                    Price = price,
                    DurationInMonths = duration,
                    IsActive = ChkIsActive.IsChecked ?? true
                };

                var response = await _tcpClient.SendRequestAsync("CreateSubscription", dto);

                HideProgressOverlay();

                if (response.IsSuccess)
                {
                    MessageBox.Show("Đã thêm Gói VIP thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    await LoadPackagesAsync();
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
                HideProgressOverlay();
                BtnAdd.IsEnabled = true;
            }
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPackageId == 0)
            {
                MessageBox.Show("Vui lòng chọn một Gói VIP để cập nhật.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateInput(out decimal price, out int duration)) return;

            BtnUpdate.IsEnabled = false;
            ShowProgressOverlay("Đang lưu dữ liệu cập nhật...");

            try
            {
                var dto = new UpdateSubscriptionDto
                {
                    Id = _selectedPackageId,
                    Name = TxtName.Text,
                    Price = price,
                    DurationInMonths = duration,
                    IsActive = ChkIsActive.IsChecked ?? true
                };

                var response = await _tcpClient.SendRequestAsync("UpdateSubscription", dto);

                HideProgressOverlay();

                if (response.IsSuccess)
                {
                    MessageBox.Show("Cập nhật Gói VIP thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    await LoadPackagesAsync();
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
                HideProgressOverlay();
                BtnUpdate.IsEnabled = true;
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPackageId == 0)
            {
                MessageBox.Show("Vui lòng chọn một Gói VIP để xóa.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa Gói VIP '{TxtName.Text}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                BtnDelete.IsEnabled = false;
                try
                {
                    var response = await _tcpClient.SendRequestAsync("DeleteSubscription", _selectedPackageId);
                    if (response.IsSuccess)
                    {
                        MessageBox.Show("Đã xóa Gói VIP thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        ClearForm();
                        await LoadPackagesAsync();
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
    }
}


