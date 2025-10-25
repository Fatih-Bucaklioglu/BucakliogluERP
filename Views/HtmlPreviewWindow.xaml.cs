using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace BucakliogluERP.Views
{
    public partial class HtmlPreviewWindow : Window
    {
        private readonly string _htmlContent;
        private readonly string _tempFilePath;

        public HtmlPreviewWindow(string htmlContent)
        {
            InitializeComponent();
            _htmlContent = htmlContent;

            // Geçici dosya oluştur
            _tempFilePath = Path.Combine(Path.GetTempPath(), $"Preview_{DateTime.Now:yyyyMMddHHmmss}.html");
            File.WriteAllText(_tempFilePath, _htmlContent, System.Text.Encoding.UTF8);

            // WebBrowser'da göster
            webBrowser.Navigate(_tempFilePath);
        }

        private void Yazdir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // WebBrowser print komutu
                webBrowser.InvokeScript("print");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yazdırma hatası: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HtmlKaydet_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "HTML Dosyası (*.html)|*.html",
                FileName = $"Siparis_{DateTime.Now:yyyyMMddHHmmss}.html"
            };

            if (saveDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveDialog.FileName, _htmlContent, System.Text.Encoding.UTF8);
                MessageBox.Show("HTML dosyası kaydedildi!", "Başarılı",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Yenile_Click(object sender, RoutedEventArgs e)
        {
            webBrowser.Refresh();
        }

        private void Kapat_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            // Geçici dosyayı temizle
            try
            {
                if (File.Exists(_tempFilePath))
                    File.Delete(_tempFilePath);
            }
            catch { }

            base.OnClosed(e);
        }
    }
}