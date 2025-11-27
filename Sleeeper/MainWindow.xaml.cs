using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sleeper
{
    public partial class MainWindow : Window
    {
        // Varsayılan süre 15 saniye (Kullanıcı değiştirebilir)
        private double _idleThreshold = 15.0;
        private CancellationTokenSource _cts;

        public MainWindow()
        {
            InitializeComponent();
        }

        // BAŞLAT BUTONU
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Arayüzü güncelle
            BtnStart.Visibility = Visibility.Collapsed;
            BtnStop.Visibility = Visibility.Visible;

            SettingsPanel.Visibility = Visibility.Collapsed; // Ayarları gizle
            TxtTimer.Visibility = Visibility.Visible;        // Sayacı göster

            // Yazılan süreyi kontrol et ve al
            if (double.TryParse(TxtInputSeconds.Text, out double result))
            {
                _idleThreshold = result;
            }

            // İzlemeyi başlat
            _cts = new CancellationTokenSource();
            Task.Run(() => MonitoringLoop(_cts.Token));
        }

        // DURDUR BUTONU
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            BtnStop.Visibility = Visibility.Collapsed;
            BtnStart.Visibility = Visibility.Visible;

            TxtTimer.Visibility = Visibility.Collapsed;    // Sayacı gizle
            SettingsPanel.Visibility = Visibility.Visible; // Ayarları geri getir

            _cts?.Cancel();
        }

        // SÜRE KUTUSUNA SADECE SAYI GİRİLMESİNİ KONTROL ET
        private void TxtInputSeconds_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Eğer boşsa veya sayı değilse varsayılan 15 yapma mantığı
            // (Burada basit bırakıyoruz, Start'a basınca kontrol ediyoruz)
        }

        // KAPATMA TUŞU (X)
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // ALTA ALMA (MİNİMİZE) TUŞU (_) - YENİ EKLENDİ
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // PENCEREYİ SÜRÜKLEME
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        // ARKA PLAN DÖNGÜSÜ
        private async Task MonitoringLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    double idleTime = IdleTimeDetector.GetIdleTimeSeconds();

                    // Kalan süreyi hesapla (Kullanıcının girdiği _idleThreshold değerine göre)
                    double remaining = _idleThreshold - idleTime;

                    if (remaining < 0) remaining = 0;

                    Dispatcher.Invoke(() =>
                    {
                        if (remaining > 0)
                            TxtTimer.Text = $"Sleeps in: {remaining:F0}s";
                        else
                            TxtTimer.Text = "Sleeping...";
                    });

                    // Süre dolduysa
                    if (idleTime > _idleThreshold)
                    {
                        MonitorControl.Sleep();

                        while (IdleTimeDetector.GetIdleTimeSeconds() > 1.0)
                        {
                            if (token.IsCancellationRequested) break;
                            await Task.Delay(1000, token);
                        }
                    }

                    await Task.Delay(200, token);
                }
                catch (TaskCanceledException) { break; }
                catch { await Task.Delay(5000, token); }
            }
        }
    }
}