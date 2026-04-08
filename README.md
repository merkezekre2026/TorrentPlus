# Torrent Client - WPF Torrent İstemcisi

Modern, qBittorrent benzeri bir Windows torrent istemcisi. .NET 8/9 ve WPF (MVVM pattern) kullanılarak geliştirilmiştir.

## 📋 Özellikler

- **Modern UI**: qBittorrent benzeri, kullanıcı dostu arayüz
- **MVVM Pattern**: Temiz kod mimarisi, test edilebilir yapı
- **Magnet Link Desteği**: Magnet linkleri ile torrent ekleme
- **Torrent Dosyası Desteği**: .torrent dosyalarını açma
- **Gerçek Zamanlı İzleme**: İndirme/yükleme hızlarını anlık takip
- **Durum Yönetimi**: Başlat, Duraklat, Durdur kontrolleri
- **Detay Paneli**: Seçili torrent'in detaylı bilgileri

## 🏗️ Proje Yapısı

```
TorrentClient/
├── Models/                 # Veri modelleri
│   └── TorrentInfo.cs      # Torrent bilgi modeli
├── ViewModels/             # MVVM ViewModel katmanı
│   └── MainViewModel.cs    # Ana pencere ViewModel'i
├── Views/                  # XAML görünümleri
│   ├── MainWindow.xaml     # Ana pencere tasarımı
│   └── MainWindow.xaml.cs  # Code-behind
├── Services/               # İş mantığı servisleri
│   └── TorrentEngineService.cs  # Torrent motoru servisi
├── Converters/             # XAML value converter'ları
│   └── Converters.cs       # Durum, hız converter'ları
├── Resources/              # Kaynak dosyalar
├── App.xaml                # Uygulama tanımlaması
├── App.xaml.cs             # Uygulama başlangıç kodu
└── TorrentClient.csproj    # Proje dosyası
```

## 🛠️ Teknolojiler

- **.NET 8/9**: Modern .NET framework
- **WPF**: Windows Presentation Foundation
- **CommunityToolkit.Mvvm**: Hafif MVVM framework
- **MonoTorrent**: Açık kaynak torrent motoru
- **ModernWpfUI**: Modern WPF kontrolleri
- **Serilog**: Loglama altyapısı

## 📦 Bağımlılıklar

Proje aşağıdaki NuGet paketlerini kullanır:

- `MonoTorrent` (v2.0.9) - Torrent protokolü motoru
- `CommunityToolkit.Mvvm` (v8.2.2) - MVVM yardımcıları
- `ModernWpfUI` (v0.9.6) - Modern UI teması
- `Serilog` (v3.1.1) - Loglama
- `Serilog.Sinks.File` (v5.0.0) - Dosya loglama
- `Serilog.Sinks.Debug` (v2.0.0) - Debug loglama

## 🚀 Kurulum ve Çalıştırma

### Gereksinimler
- Windows 10/11
- .NET 8.0 SDK veya üzeri
- Visual Studio 2022 veya VS Code

### Adımlar

1. **Projeyi Klonlayın**
```bash
cd /workspace/TorrentClient
```

2. **Bağımlılıkları Yükleyin**
```bash
dotnet restore
```

3. **Projeyi Derleyin**
```bash
dotnet build
```

4. **Uygulamayı Çalıştırın**
```bash
dotnet run
```

## 🎯 Kullanım

### Magnet Link Ekleme
1. "➕ Magnet Ekle" butonuna tıklayın
2. Magnet linkini yapıştırın
3. İndirme otomatik başlayacaktır

### Torrent Dosyası Ekleme
1. "📁 Dosya Ekle" butonuna tıklayın
2. .torrent dosyasını seçin
3. İndirme otomatik başlayacaktır

### Kontroller
- **▶ Başlat**: Seçili torrent'i başlatır
- **⏸ Duraklat**: Seçili torrent'i duraklatır
- **⏹ Durdur**: Seçili torrent'i tamamen durdurur
- **🗑 Kaldır**: Torrent'i listeden kaldırır

## 📊 UI Bölümleri

### Toolbar (Üst)
- Torrent ekleme butonları (Magnet/Dosya)
- Kontrol butonları (Başlat/Duraklat/Durdur)
- Kaldır butonu

### Torrent Listesi (Orta)
- Ad, İlerleme, Hız, Boyut, Peers/Seeds, Durum kolonları
- Progress bar ile görsel ilerleme göstergesi
- Renkli durum etiketleri

### Detay Paneli (Alt)
- Genel bilgiler (Ad, Durum, İlerleme, Eklenme tarihi)
- Hız bilgileri (İndirme/Yükleme)
- Bağlantı bilgileri (Peers, Seeds)

### Status Bar (En Alt)
- Durum mesajları
- Aktif torrent sayısı
- Toplam indirme/yükleme hızı

## 🔧 Geliştirme

### Yeni Özellik Ekleme

1. **Model**: `Models/` klasörüne yeni model sınıfı ekleyin
2. **ViewModel**: `ViewModels/` klasöründe ilgili ViewModel'i güncelleyin
3. **View**: `Views/` klasöründe XAML'i düzenleyin
4. **Converter**: Gerekirse `Converters/` klasörüne yeni converter ekleyin

### Kod Standartları
- Async/await kullanımı (asenkron işlemler için)
- XML dokümantasyon (public member'lar için)
- CommunityToolkit.Mvvm source generators kullanımı
- Exception handling ve loglama

## ⚠️ Önemli Notlar

- Bu proje eğitim amaçlı geliştirilmiştir
- Yasal içerikleri indirmek için kullanın
- Torrent protokolü P2P bağlantıları gerektirir (güvenlik duvarı ayarları gerekebilir)
- Windows platformu için tasarlanmıştır (WPF nedeniyle)

## 📝 Lisans

Bu proje açık kaynak olarak geliştirilmektedir.

## 🤝 Katkıda Bulunma

1. Projeyi fork edin
2. Feature branch oluşturun (`git checkout -b feature/YeniOzellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik eklendi'`)
4. Branch'i push edin (`git push origin feature/YeniOzellik`)
5. Pull Request oluşturun

---

**Geliştirici**: Senior Full-Stack Engineer  
**Tarih**: 2024  
**Teknoloji**: .NET 8, WPF, C#, MonoTorrent
