using System;

namespace TorrentClient.Models
{
    /// <summary>
    /// Torrent indirme durumlarını temsil eden enum
    /// </summary>
    public enum TorrentState
    {
        NotStarted,
        Downloading,
        Seeding,
        Paused,
        Stopped,
        Error,
        Checking,
        Queued
    }

    /// <summary>
    /// Tek bir torrent dosyasının bilgilerini tutan model sınıfı
    /// </summary>
    public class TorrentInfo : ObservableObject
    {
        private string _name = string.Empty;
        private string _hash = string.Empty;
        private string _magnetLink = string.Empty;
        private long _totalSize;
        private long _downloadedBytes;
        private double _progress;
        private double _downloadSpeed;
        private double _uploadSpeed;
        private int _peers;
        private int _seeds;
        private TorrentState _state;
        private DateTime? _addedDate;
        private DateTime? _completedDate;

        /// <summary>
        /// Torrent adı
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Torrent info hash (benzersiz kimlik)
        /// </summary>
        public string Hash
        {
            get => _hash;
            set => SetProperty(ref _hash, value);
        }

        /// <summary>
        /// Magnet linki
        /// </summary>
        public string MagnetLink
        {
            get => _magnetLink;
            set => SetProperty(ref _magnetLink, value);
        }

        /// <summary>
        /// Toplam boyut (byte)
        /// </summary>
        public long TotalSize
        {
            get => _totalSize;
            set => SetProperty(ref _totalSize, value);
        }

        /// <summary>
        /// İndirilen boyut (byte)
        /// </summary>
        public long DownloadedBytes
        {
            get => _downloadedBytes;
            set
            {
                SetProperty(ref _downloadedBytes, value);
                OnPropertyChanged(nameof(DownloadedSizeFormatted));
            }
        }

        /// <summary>
        /// İndirme ilerleme yüzdesi (0-100)
        /// </summary>
        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        /// <summary>
        /// İndirme hızı (bytes/s)
        /// </summary>
        public double DownloadSpeed
        {
            get => _downloadSpeed;
            set
            {
                SetProperty(ref _downloadSpeed, value);
                OnPropertyChanged(nameof(DownloadSpeedFormatted));
            }
        }

        /// <summary>
        /// Yükleme hızı (bytes/s)
        /// </summary>
        public double UploadSpeed
        {
            get => _uploadSpeed;
            set
            {
                SetProperty(ref _uploadSpeed, value);
                OnPropertyChanged(nameof(UploadSpeedFormatted));
            }
        }

        /// <summary>
        /// Bağlı peer sayısı
        /// </summary>
        public int Peers
        {
            get => _peers;
            set => SetProperty(ref _peers, value);
        }

        /// <summary>
        /// Seed sayısı
        /// </summary>
        public int Seeds
        {
            get => _seeds;
            set => SetProperty(ref _seeds, value);
        }

        /// <summary>
        /// Torrent durumu
        /// </summary>
        public TorrentState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        /// <summary>
        /// Eklenme tarihi
        /// </summary>
        public DateTime? AddedDate
        {
            get => _addedDate;
            set => SetProperty(ref _addedDate, value);
        }

        /// <summary>
        /// Tamamlanma tarihi
        /// </summary>
        public DateTime? CompletedDate
        {
            get => _completedDate;
            set => SetProperty(ref _completedDate, value);
        }

        // Formatlanmış özellikler (UI için)
        public string DownloadedSizeFormatted => FormatFileSize(DownloadedBytes);
        public string TotalSizeFormatted => FormatFileSize(TotalSize);
        public string DownloadSpeedFormatted => FormatSpeed(DownloadSpeed);
        public string UploadSpeedFormatted => FormatSpeed(UploadSpeed);
        public string ProgressFormatted => $"{Progress:F1}%";
        public string StateFormatted => State.ToString();

        /// <summary>
        /// Dosya boyutunu insan tarafından okunabilir formata çevirir
        /// </summary>
        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }

        /// <summary>
        /// Hızı insan tarafından okunabilir formata çevirir
        /// </summary>
        private static string FormatSpeed(double bytesPerSecond)
        {
            string[] sizes = { "B/s", "KB/s", "MB/s", "GB/s" };
            int order = 0;
            double speed = bytesPerSecond;
            while (speed >= 1024 && order < sizes.Length - 1)
            {
                order++;
                speed /= 1024;
            }
            return $"{speed:0.##} {sizes[order]}";
        }
    }
}
