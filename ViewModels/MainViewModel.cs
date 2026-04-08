using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using TorrentClient.Models;
using TorrentClient.Services;

namespace TorrentClient.ViewModels
{
    /// <summary>
    /// Ana pencere için ViewModel sınıfı
    /// MVVM pattern'e uygun olarak UI ve business logic'i ayırır
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private readonly TorrentEngineService _torrentService;
        private bool _isInitialized;

        [ObservableProperty]
        private string _windowTitle = "Torrent Client - qBittorrent Tarzı";

        [ObservableProperty]
        private ObservableCollection<TorrentInfo> _torrents = new();

        [ObservableProperty]
        private TorrentInfo? _selectedTorrent;

        [ObservableProperty]
        private string _statusBarMessage = "Hazır";

        [ObservableProperty]
        private double _totalDownloadSpeed;

        [ObservableProperty]
        private double _totalUploadSpeed;

        [ObservableProperty]
        private int _activeTorrentCount;

        [ObservableProperty]
        private bool _isEngineReady;

        /// <summary>
        /// Yapıcı metod - Servisi başlatır
        /// </summary>
        public MainViewModel()
        {
            var downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\Torrents";
            _torrentService = new TorrentEngineService(downloadPath);
            
            // Servis olaylarını abone ol
            _torrentService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_torrentService.Torrents))
                {
                    Torrents = _torrentService.Torrents;
                }
                else if (e.PropertyName == nameof(_torrentService.SelectedTorrent))
                {
                    SelectedTorrent = _torrentService.SelectedTorrent;
                }
                else if (e.PropertyName == nameof(_torrentService.IsEngineReady))
                {
                    IsEngineReady = _torrentService.IsEngineReady;
                }
                else if (e.PropertyName == nameof(_torrentService.StatusMessage))
                {
                    StatusBarMessage = _torrentService.StatusMessage;
                }
            };

            InitializeCommand = new AsyncRelayCommand(InitializeAsync);
            AddMagnetLinkCommand = new AsyncRelayCommand<string>(AddMagnetLinkAsync);
            AddTorrentFileCommand = new AsyncRelayCommand(AddTorrentFileAsync);
            StartTorrentCommand = new AsyncRelayCommand(StartSelectedTorrentAsync);
            PauseTorrentCommand = new AsyncRelayCommand(PauseSelectedTorrentAsync);
            StopTorrentCommand = new AsyncRelayCommand(StopSelectedTorrentAsync);
            RemoveTorrentCommand = new AsyncRelayCommand(RemoveSelectedTorrentAsync);
        }

        #region Commands

        public IAsyncRelayCommand InitializeCommand { get; }
        public IAsyncRelayCommand<string> AddMagnetLinkCommand { get; }
        public IAsyncRelayCommand AddTorrentFileCommand { get; }
        public IAsyncRelayCommand StartTorrentCommand { get; }
        public IAsyncRelayCommand PauseTorrentCommand { get; }
        public IAsyncRelayCommand StopTorrentCommand { get; }
        public IAsyncRelayCommand RemoveTorrentCommand { get; }

        #endregion

        #region Command Methods

        /// <summary>
        /// Torrent motorunu başlatır
        /// </summary>
        private async Task InitializeAsync()
        {
            if (_isInitialized) return;

            try
            {
                StatusBarMessage = "Motor başlatılıyor...";
                await _torrentService.InitializeAsync();
                _isInitialized = true;
                StatusBarMessage = "Motor hazır - Kullanıma hazır";
            }
            catch (Exception ex)
            {
                StatusBarMessage = $"Hata: {ex.Message}";
                MessageBox.Show($"Motor başlatma hatası:\n{ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Magnet linki ekler
        /// </summary>
        private async Task AddMagnetLinkAsync(string? magnetLink)
        {
            if (string.IsNullOrWhiteSpace(magnetLink))
            {
                // Dialog aç
                var dialog = new MagnetLinkDialog();
                if (dialog.ShowDialog() == true)
                {
                    magnetLink = dialog.MagnetLink;
                }
                else
                {
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(magnetLink))
            {
                try
                {
                    StatusBarMessage = "Magnet linki ekleniyor...";
                    await _torrentService.AddMagnetLinkAsync(magnetLink);
                    UpdateTotals();
                    StatusBarMessage = "Torrent eklendi";
                }
                catch (Exception ex)
                {
                    StatusBarMessage = $"Hata: {ex.Message}";
                    MessageBox.Show($"Magnet ekleme hatası:\n{ex.Message}", "Hata",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// .torrent dosyası ekler
        /// </summary>
        private async Task AddTorrentFileAsync()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Torrent Dosyaları (*.torrent)|*.torrent|Tüm Dosyalar (*.*)|*.*",
                Title = "Torrent Dosyası Seç"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    StatusBarMessage = "Torrent dosyası ekleniyor...";
                    await _torrentService.AddTorrentFileAsync(openFileDialog.FileName);
                    UpdateTotals();
                    StatusBarMessage = "Torrent dosyası eklendi";
                }
                catch (Exception ex)
                {
                    StatusBarMessage = $"Hata: {ex.Message}";
                    MessageBox.Show($"Torrent dosyası ekleme hatası:\n{ex.Message}", "Hata",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Seçili torrent'i başlatır
        /// </summary>
        private async Task StartSelectedTorrentAsync()
        {
            if (SelectedTorrent != null)
            {
                try
                {
                    await _torrentService.StartTorrentAsync(SelectedTorrent);
                    UpdateTotals();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Başlatma hatası:\n{ex.Message}", "Hata",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Seçili torrent'i duraklatır
        /// </summary>
        private async Task PauseSelectedTorrentAsync()
        {
            if (SelectedTorrent != null)
            {
                try
                {
                    await _torrentService.PauseTorrentAsync(SelectedTorrent);
                    UpdateTotals();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Duraklatma hatası:\n{ex.Message}", "Hata",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Seçili torrent'i durdurur
        /// </summary>
        private async Task StopSelectedTorrentAsync()
        {
            if (SelectedTorrent != null)
            {
                try
                {
                    await _torrentService.StopTorrentAsync(SelectedTorrent);
                    UpdateTotals();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Durdurma hatası:\n{ex.Message}", "Hata",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Seçili torrent'i listeden kaldırır
        /// </summary>
        private async Task RemoveSelectedTorrentAsync()
        {
            if (SelectedTorrent != null)
            {
                var result = MessageBox.Show(
                    $"'{SelectedTorrent.Name}' torrent'ini listeden kaldırmak istediğinize emin misiniz?",
                    "Onayla",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _torrentService.StopTorrentAsync(SelectedTorrent);
                        Torrents.Remove(SelectedTorrent);
                        SelectedTorrent = null;
                        UpdateTotals();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Kaldırma hatası:\n{ex.Message}", "Hata",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Toplam indirme/yükleme hızlarını ve aktif torrent sayısını günceller
        /// </summary>
        private void UpdateTotals()
        {
            TotalDownloadSpeed = Torrents.Sum(t => t.DownloadSpeed);
            TotalUploadSpeed = Torrents.Sum(t => t.UploadSpeed);
            ActiveTorrentCount = Torrents.Count(t => 
                t.State == TorrentState.Downloading || t.State == TorrentState.Seeding);
        }

        #endregion
    }

    /// <summary>
    /// Magnet linki giriş dialog'u için basit bir sınıf
    /// </summary>
    public class MagnetLinkDialog
    {
        public string? MagnetLink { get; set; }

        public bool? ShowDialog()
        {
            // Basit implementation - gerçek uygulamada WPF Window kullanılacak
            var input = Microsoft.VisualBasic.Interaction.InputBox(
                "Magnet linkini girin:", 
                "Magnet Linki Ekle", 
                "");

            if (!string.IsNullOrWhiteSpace(input))
            {
                MagnetLink = input.Trim();
                return true;
            }

            return false;
        }
    }
}
