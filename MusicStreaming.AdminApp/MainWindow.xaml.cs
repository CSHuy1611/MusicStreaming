using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MusicStreaming.AdminApp.Views;

namespace MusicStreaming.AdminApp
{
    public partial class MainWindow : Window
    {
        // Khai báo màu Nền
        private readonly SolidColorBrush _defaultBgColor;
        private readonly SolidColorBrush _activeBgColor;

        // Khai báo màu Viền (Border)
        private readonly SolidColorBrush _defaultBorderColor;
        private readonly SolidColorBrush _activeBorderColor;

        public MainWindow()
        {
            InitializeComponent();

            // Màu nền
            _defaultBgColor = new SolidColorBrush(Colors.Transparent);
            _activeBgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#282828")); // Nền xám đen

            // Màu viền bao quanh tab
            _defaultBorderColor = new SolidColorBrush(Colors.Transparent);
            _activeBorderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#404040")); // Viền xám sáng hơn nền một chút

            // Mặc định load View và Highlight tab Dashboard khi khởi động
            LoadView(new DashboardControl());
            HighlightTab(BtnDashboard);
        }

        private void HighlightTab(Button activeButton)
        {
            // 1. Reset toàn bộ màu nền và màu viền các nút về mặc định
            BtnDashboard.Background = _defaultBgColor;
            BtnDashboard.BorderBrush = _defaultBorderColor;

            BtnManageSongs.Background = _defaultBgColor;
            BtnManageSongs.BorderBrush = _defaultBorderColor;

            BtnManageSingers.Background = _defaultBgColor;
            BtnManageSingers.BorderBrush = _defaultBorderColor;

            BtnManageGenres.Background = _defaultBgColor;
            BtnManageGenres.BorderBrush = _defaultBorderColor;

            BtnManageSubscriptions.Background = _defaultBgColor;
            BtnManageSubscriptions.BorderBrush = _defaultBorderColor;

            // 2. Cấp màu nền và viền bao quanh cho tab đang được chọn
            if (activeButton != null)
            {
                activeButton.Background = _activeBgColor;
                activeButton.BorderBrush = _activeBorderColor;
            }
        }

        private void LoadView(System.Windows.Controls.UserControl view)
        {
            MainContentControl.Content = view;
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            HighlightTab((Button)sender);
            LoadView(new DashboardControl());
        }

        private void BtnManageSongs_Click(object sender, RoutedEventArgs e)
        {
            HighlightTab((Button)sender);
            LoadView(new SongManagementControl());
        }

        private void BtnManageSingers_Click(object sender, RoutedEventArgs e)
        {
            HighlightTab((Button)sender);
            LoadView(new SingerManagementControl());
        }

        private void BtnManageGenres_Click(object sender, RoutedEventArgs e)
        {
            HighlightTab((Button)sender);
            LoadView(new LibraryManagementControl());
        }
        private void BtnManageSubscriptions_Click(object sender, RoutedEventArgs e)
        {
            HighlightTab((Button)sender);
            LoadView(new SubscriptionManagementControl());
        }
    }
}