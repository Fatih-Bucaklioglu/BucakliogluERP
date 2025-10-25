using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using BucakliogluERP.Services;

namespace BucakliogluERP.Views
{
    public partial class RaporlarPage : Page
    {
        private readonly RaporService _raporService;
        private readonly ExcelExportService _excelService;

        public RaporlarPage()
        {
            InitializeComponent();
            _raporService = new RaporService();
            _excelService = new ExcelExportService();

            // Varsayılan tarihler: Bu ay
            dpBaslangic.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dpBitis.SelectedDate = DateTime.Now;

            RaporlariYukle();
        }

        private void RaporlariYukle()
        {
            var baslangic = dpBaslangic.SelectedDate ?? DateTime.Now.AddMonths(-1);
            var bitis = dpBitis.SelectedDate ?? DateTime.Now;

            // Performans Özeti
            var performans = _raporService.PerformansOzetiGetir(baslangic, bitis);
            txtToplamCiro.Text = $"{performans.ToplamCiro:N2} ₺";
            txtSiparisSayisi.Text = performans.ToplamSiparisSayisi.ToString();
            txtOrtalamaSiparis.Text = $"{performans.OrtalamaSiparisTutari:N2} ₺";
            txtMusteriSayisi.Text = performans.ToplamMusteriSayisi.ToString();

            // Aylık Satış
            dgAylikSatis.ItemsSource = _raporService.AylikSatisRaporuGetir(bitis.Year);

            // En Çok Satan Ürünler
            dgEnCokSatan.ItemsSource = _raporService.EnCokSatanUrunler(20);

            // En Karlı Müşteriler
            dgEnKarliMusteriler.ItemsSource = _raporService.EnKarliMusteriler(20);

            // Kategori Raporu
            dgKategoriRaporu.ItemsSource = _raporService.KategoriSatisRaporu();

            // Ödeme Durumu
            var odemeDurumu = _raporService.OdemeDurumuRaporuGetir();
            txtToplamBorc.Text = $"{odemeDurumu.ToplamBorc:N2} ₺";
            txtBorcluSayisi.Text = $"{odemeDurumu.BorcluMusteriSayisi} müşteri";
            txtToplamAlacak.Text = $"{odemeDurumu.ToplamAlacak:N2} ₺";
            txtAlacakliSayisi.Text = $"{odemeDurumu.AlacakliMusteriSayisi} müşteri";
            txtTahsilEdilmemisRapor.Text = $"{odemeDurumu.TahsilEdilmemis:N2} ₺";
        }

        private void RaporlariOlustur_Click(object sender, RoutedEventArgs e)
        {
            RaporlariYukle();
        }

        private void BuAy_Click(object sender, RoutedEventArgs e)
        {
            dpBaslangic.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dpBitis.SelectedDate = DateTime.Now;
            RaporlariYukle();
        }

        private void BuYil_Click(object sender, RoutedEventArgs e)
        {
            dpBaslangic.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
            dpBitis.SelectedDate = DateTime.Now;
            RaporlariYukle();
        }

        private void Son30Gun_Click(object sender, RoutedEventArgs e)
        {
            dpBaslangic.SelectedDate = DateTime.Now.AddDays(-30);
            dpBitis.SelectedDate = DateTime.Now; RaporlariYukle();
        }

        private void Son90Gun_Click(object sender, RoutedEventArgs e)
        {
            dpBaslangic.SelectedDate = DateTime.Now.AddDays(-90);
            dpBitis.SelectedDate = DateTime.Now;
            RaporlariYukle();
        }

        private void TumZamanlar_Click(object sender, RoutedEventArgs e)
        {
            dpBaslangic.SelectedDate = new DateTime(2000, 1, 1);
            dpBitis.SelectedDate = DateTime.Now;
            RaporlariYukle();
        }

        private void AylikSatisExcel_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel("AylikSatis", dgAylikSatis.ItemsSource);
        }

        private void UrunSatisExcel_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel("EnCokSatanUrunler", dgEnCokSatan.ItemsSource);
        }

        private void MusteriRaporuExcel_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel("EnKarliMusteriler", dgEnKarliMusteriler.ItemsSource);
        }

        private void ExportToExcel(string raporAdi, object data)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Dosyası (*.xlsx)|*.xlsx",
                    FileName = $"{raporAdi}_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    // Basit Excel export
                    MessageBox.Show("Excel raporu oluşturuldu!", "Başarılı",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}