using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonoTorrent;
using MonoTorrent.Client;
using TorrentClient.Models;

namespace TorrentClient.Services
{
    /// <summary>
    /// Torrent işlemlerini yöneten ana servis sınıfı
    /// MonoTorrent kütüphanesini kullanarak torrent indirme/yükleme işlemlerini gerçekleştirir
    /// </summary>
    public partial class TorrentEngineService : ObservableObject, IDisposable
    {
        private ClientEngine? _engine;
        private TorrentManager? _currentManager;
        private CancellationTokenSource? _updateCts;
        private bool _isDisposed;
        private readonly string _downloadPath;
        private readonly ILogger _logger;

        [ObservableProperty]
        private ObservableCollection<TorrentInfo> _torrents = new();

        [ObservableProperty]
        private TorrentInfo? _selectedTorrent;

        [ObservableProperty]
        private bool _isEngineReady;

        [ObservableProperty]
        private string _statusMessage = "Motor başlatılıyor...";

        /// <summary>
        /// Servis yapıcı metodu
        /// </summary>
        /// <param name="downloadPath">İndirme klasörü yolu</param>
        /// <param name="logger">Loglama servisi (opsiyonel)</param>
        public TorrentEngineService(string downloadPath, ILogger? logger = null)
        {
            _downloadPath = downloadPath;
            _logger = logger ?? new NullLogger();
            
            // İndirme klasörünü oluştur
            if (!Directory.Exists(_downloadPath))
            {
                Directory.CreateDirectory(_downloadPath);
            }
        }

        /// <summary>
        /// Torrent motorunu başlatır
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _logger.Info("Torrent motoru başlatılıyor...");
                
                // ClientEngine ayarları
                var settings = new EngineSettings
                {
                    CacheDirectory = _downloadPath,
                    ListenEndPoints = { new System.Net.IPEndPoint(System.Net.IPAddress.Any, 51413) }
                };

                _engine = new ClientEngine(settings);
                IsEngineReady = true;
                StatusMessage = "Motor hazır";
                
                _logger.Info("Torrent motoru başarıyla başlatıldı");
            }
            catch (Exception ex)
            {
                _logger.Error($"Motor başlatma hatası: {ex.Message}");
                StatusMessage = $"Hata: {ex.Message}";
                throw;
            }
        }

        /// <summary>
        /// Magnet linki ile torrent ekler ve indirmeyi başlatır
        /// </summary>
        public async Task AddMagnetLinkAsync(string magnetLink)
        {
            if (_engine == null || !IsEngineReady)
                throw new InvalidOperationException("Motor henüz hazır değil");

            try
            {
                _logger.Info($"Magnet linki ekleniyor: {magnetLink.Substring(0, Math.Min(50, magnetLink.Length))}...");
                StatusMessage = "Magnet linki işleniyor...";

                // Magnet linkinden torrent metadata'sını yükle
                var torrent = await Torrent.LoadAsync(magnetLink);
                
                var torrentManager = await _engine.AddAsync(torrent, _downloadPath);
                
                var torrentInfo = new TorrentInfo
                {
                    Name = torrent.Name ?? "Bilinmeyen",
                    Hash = torrent.InfoHashes.V1OrV2.ToString(),
                    MagnetLink = magnetLink,
                    TotalSize = torrent.Size,
                    State = TorrentState.NotStarted,
                    AddedDate = DateTime.Now
                };

                Torrents.Add(torrentInfo);
                SelectedTorrent = torrentInfo;

                // İndirmeyi başlat
                await StartTorrentAsync(torrentInfo);

                _logger.Info($"Torrent eklendi: {torrentInfo.Name}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Magnet ekleme hatası: {ex.Message}");
                StatusMessage = $"Hata: {ex.Message}";
                throw;
            }
        }

        /// <summary>
        /// .torrent dosyasından torrent ekler
        /// </summary>
        public async Task AddTorrentFileAsync(string filePath)
        {
            if (_engine == null || !IsEngineReady)
                throw new InvalidOperationException("Motor henüz hazır değil");

            try
            {
                _logger.Info($"Torrent dosyası ekleniyor: {filePath}");
                StatusMessage = "Torrent dosyası yükleniyor...";

                var torrent = await Torrent.LoadAsync(filePath);
                
                var torrentManager = await _engine.AddAsync(torrent, _downloadPath);
                
                var torrentInfo = new TorrentInfo
                {
                    Name = torrent.Name ?? Path.GetFileNameWithoutExtension(filePath),
                    Hash = torrent.InfoHashes.V1OrV2.ToString(),
                    TotalSize = torrent.Size,
                    State = TorrentState.NotStarted,
                    AddedDate = DateTime.Now
                };

                Torrents.Add(torrentInfo);
                SelectedTorrent = torrentInfo;

                await StartTorrentAsync(torrentInfo);

                _logger.Info($"Torrent dosyası eklendi: {torrentInfo.Name}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Torrent dosyası ekleme hatası: {ex.Message}");
                StatusMessage = $"Hata: {ex.Message}";
                throw;
            }
        }

        /// <summary>
        /// Belirtilen torrent'i başlatır
        /// </summary>
        public async Task StartTorrentAsync(TorrentInfo torrentInfo)
        {
            if (_engine == null) return;

            try
            {
                var manager = await _engine.FindAsync(torrentInfo.Hash);
                if (manager != null)
                {
                    await manager.StartAsync();
                    torrentInfo.State = TorrentState.Downloading;
                    
                    // Durum güncellemelerini başlat
                    StartStatusUpdates(manager, torrentInfo);
                    
                    _logger.Info($"Torrent başlatıldı: {torrentInfo.Name}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Torrent başlatma hatası: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Belirtilen torrent'i duraklatır
        /// </summary>
        public async Task PauseTorrentAsync(TorrentInfo torrentInfo)
        {
            try
            {
                var manager = await _engine!.FindAsync(torrentInfo.Hash);
                if (manager != null)
                {
                    await manager.StopAsync();
                    torrentInfo.State = TorrentState.Paused;
                    _logger.Info($"Torrent duraklatıldı: {torrentInfo.Name}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Torrent duraklatma hatası: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Belirtilen torrent'i tamamen durdurur
        /// </summary>
        public async Task StopTorrentAsync(TorrentInfo torrentInfo)
        {
            try
            {
                var manager = await _engine!.FindAsync(torrentInfo.Hash);
                if (manager != null)
                {
                    await manager.StopAsync();
                    torrentInfo.State = TorrentState.Stopped;
                    _logger.Info($"Torrent durduruldu: {torrentInfo.Name}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Torrent durdurma hatası: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Torrent durumunu sürekli günceller
        /// </summary>
        private void StartStatusUpdates(TorrentManager manager, TorrentInfo torrentInfo)
        {
            _updateCts?.Cancel();
            _updateCts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (!_updateCts.Token.IsCancellationRequested)
                {
                    try
                    {
                        var stats = manager.Monitor;
                        
                        // UI thread'de güncelle
                        await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            torrentInfo.Progress = stats.Progress * 100;
                            torrentInfo.DownloadedBytes = stats.BytesDownloaded;
                            torrentInfo.DownloadSpeed = stats.DownloadRate;
                            torrentInfo.UploadSpeed = stats.UploadRate;
                            torrentInfo.Peers = stats.ActivePeers;
                            torrentInfo.Seeds = stats.ConnectedSeeders;

                            // Durum güncellemesi
                            torrentInfo.State = manager.State switch
                            {
                                TorrentState.Downloading => Models.TorrentState.Downloading,
                                TorrentState.Seeding => Models.TorrentState.Seeding,
                                TorrentState.Stopped => Models.TorrentState.Stopped,
                                TorrentState.Paused => Models.TorrentState.Paused,
                                _ => torrentInfo.State
                            };

                            // Tamamlandı mı kontrol et
                            if (stats.Progress >= 1.0 && torrentInfo.State != Models.TorrentState.Seeding)
                            {
                                torrentInfo.State = Models.TorrentState.Seeding;
                                torrentInfo.CompletedDate = DateTime.Now;
                                _logger.Info($"Torrent tamamlandı: {torrentInfo.Name}");
                            }
                        });

                        await Task.Delay(1000, _updateCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Durum güncelleme hatası: {ex.Message}");
                        break;
                    }
                }
            }, _updateCts.Token);
        }

        /// <summary>
        /// Servisi temizler ve kaynakları serbest bırakır
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            _updateCts?.Cancel();
            _updateCts?.Dispose();
            
            _engine?.Dispose();
            
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Basit logger interface'i
    /// </summary>
    public interface ILogger
    {
        void Info(string message);
        void Error(string message);
        void Warning(string message);
        void Debug(string message);
    }

    /// <summary>
    /// Boş logger implementasyonu (null object pattern)
    /// </summary>
    public class NullLogger : ILogger
    {
        public void Info(string message) => System.Diagnostics.Debug.WriteLine($"[INFO] {message}");
        public void Error(string message) => System.Diagnostics.Debug.WriteLine($"[ERROR] {message}");
        public void Warning(string message) => System.Diagnostics.Debug.WriteLine($"[WARN] {message}");
        public void Debug(string message) => System.Diagnostics.Debug.WriteLine($"[DEBUG] {message}");
    }
}
