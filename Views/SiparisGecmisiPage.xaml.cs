using BucakliogluERP.Models;
using BucakliogluERP.Services;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BucakliogluERP.Views
{
    public partial class SiparisGecmisiPage : Page
    {
        private readonly DataService _dataService;

        public SiparisGecmisiPage()
        {
            InitializeComponent();
            _dataService = new DataService();
            SiparisListesiniYukle();
        }

        private void SiparisListesiniYukle(string aramaMetni = "")
        {
            var siparisler = _dataService.LoadSiparisler();

            if (!string.IsNullOrWhiteSpace(aramaMetni))
            {
                siparisler = siparisler.Where(s =>
                    s.SiparisNo.Contains(aramaMetni, StringComparison.OrdinalIgnoreCase) ||
                    (s.Musteri?.Muhattap?.Contains(aramaMetni, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            var siparisListesi = siparisler.Select(s => new
            {
                s.Id,
                s.SiparisNo,
                s.Tarih,
                s.Musteri,
                s.GenelToplam,
                UrunSayisi = s.Urunler.Count,
                Siparis = s
            }).OrderByDescending(s => s.Tarih).ToList();

            lstSiparisler.ItemsSource = siparisListesi;

            bosPanel.Visibility = siparisListesi.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TxtArama_TextChanged(object sender, TextChangedEventArgs e)
        {
            SiparisListesiniYukle(txtArama.Text);
        }

        private void SiparisDetay_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext != null)
            {
                var item = border.DataContext;
                var property = item.GetType().GetProperty("Siparis");
                var siparis = property?.GetValue(item);

                if (siparis != null)
                {
                    MessageBox.Show("Sipariş detay sayfası yakında eklenecek!", "Bilgi",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void TumunuExport_Click(object sender, RoutedEventArgs e)
        {
            var siparisler = _dataService.LoadSiparisler();

            if (siparisler.Count == 0)
            {
                MessageBox.Show("Henüz sipariş bulunmuyor!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Dosyası (*.xlsx)|*.xlsx",
                    FileName = $"TumSiparisler_{DateTime.Now:yyyyMMdd}.xlsx",
                    Title = "Excel Olarak Kaydet"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var excelService = new ExcelExportService();
                    excelService.TumSiparislerExport(siparisler, saveDialog.FileName);

                    MessageBox.Show($"{siparisler.Count} sipariş Excel'e aktarıldı!", "Başarılı",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveDialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excel oluştururken hata: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // SiparisGecmisiPage.xaml.cs içinde
        private void SiparisDetay_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext != null)
            {
                var item = border.DataContext;
                var property = item.GetType().GetProperty("Siparis");
                var siparis = property?.GetValue(item) as Siparis;

                if (siparis != null)
                {
                    var detayWindow = new SiparisDetayWindow(siparis);
                    detayWindow.ShowDialog();
                }
            }
        }
        private void TopluYazdir_Click(object sender, RoutedEventArgs e)
        {
            var topluYazdirWindow = new TopluYazdirmaWindow
            {
                Owner = Window.GetWindow(this)
            };
            topluYazdirWindow.ShowDialog();
        }
    }
}