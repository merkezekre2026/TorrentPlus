using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TorrentClient.Models;

namespace TorrentClient.Converters
{
    /// <summary>
    /// Torrent durumuna göre arka plan rengi döndüren converter
    /// </summary>
    public class StateToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TorrentState state)
            {
                return state switch
                {
                    TorrentState.Downloading => "#E3F2FD",  // Mavi açık
                    TorrentState.Seeding => "#E8F5E9",       // Yeşil açık
                    TorrentState.Paused => "#FFF3E0",        // Turuncu açık
                    TorrentState.Error => "#FFEBEE",         // Kırmızı açık
                    TorrentState.Stopped => "#F5F5F5",       // Gri
                    _ => "#E3F2FD"
                };
            }
            return "#E3F2FD";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Torrent durumuna göre metin rengi döndüren converter
    /// </summary>
    public class StateToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TorrentState state)
            {
                return state switch
                {
                    TorrentState.Downloading => "#1976D2",  // Mavi
                    TorrentState.Seeding => "#388E3C",       // Yeşil
                    TorrentState.Paused => "#F57C00",        // Turuncu
                    TorrentState.Error => "#D32F2F",         // Kırmızı
                    TorrentState.Stopped => "#757575",       // Gri
                    _ => "#1976D2"
                };
            }
            return "#1976D2";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Byte/s hız değerini okunabilir formata çeviren converter
    /// </summary>
    public class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double bytesPerSecond)
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
            return "0 B/s";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Boolean değeri görünürlüğe çeviren converter (Collapsed/Visible)
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }
}
