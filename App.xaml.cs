using System;
using System.Windows;

namespace TorrentClient
{
    /// <summary>
    /// Uygulama başlangıç noktası ve merkezi loglama yapılandırması
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Loglama yapılandırması
            ConfigureLogging();
            
            // Unhandled exception handler
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void ConfigureLogging()
        {
            // Serilog konfigürasyonu burada yapılacak
            // Şimdilik basit debug output
            System.Diagnostics.Debug.WriteLine("Application started - Logging initialized");
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"UI Thread Exception: {e.Exception.Message}");
            MessageBox.Show($"Bir hata oluştu: {e.Exception.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Background Thread Exception: {e.ExceptionObject}");
        }
    }
}
