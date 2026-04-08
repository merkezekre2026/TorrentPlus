using System.Windows;
using TorrentClient.ViewModels;

namespace TorrentClient.Views
{
    /// <summary>
    /// MainWindow.xaml için code-behind dosyası
    /// DataContext'i MainViewModel olarak ayarlar ve başlangıç işlemlerini yapar
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            
            // ViewModel oluştur ve DataContext'e ata
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            
            // Pencere yüklendiğinde motoru başlat
            Loaded += MainWindow_Loaded;
            
            // Pencere kapanırken kaynakları temizle
            Closed += MainWindow_Closed;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Torrent motorunu başlat
            await _viewModel.InitializeCommand.ExecuteAsync(null);
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            // Servis kaynaklarını temizle
            // Not: IDisposable implementasyonu eklenebilir
        }
    }
}
