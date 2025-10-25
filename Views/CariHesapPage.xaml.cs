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
    public partial class CariHesapPage : Page
    {
        private readonly DataService _dataService;
        private CariHesap? _secilenCari;

        public CariHesapPage()
        {
            InitializeComponent();
            _dataService = new DataService();
            CariListesiniYukle();
        }

        private void CariListesiniYukle()
        {
            var cariHesaplar = _dataService.LoadCariHesaplar();

            var cariListesi = cariHesaplar.Select(c => new
            {
                c.Id,
                c.Musteri,
                c.Telefon,
                c.Bakiye,
                BakiyeNegatif = c.Bakiye < 0,
                CariHesap = c
            }).ToList();

            lstCariHesaplar.ItemsSource = cariListesi;
        }

        private void CariSecim_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext != null)
            {
                var item = border.DataContext;
                var property = item.GetType().GetProperty("CariHesap");
                _secilenCari = property?.GetValue(item) as CariHesap;

                if (_secilenCari != null)
                {
                    CariDetayGoster();
                }
            }
        }

        private void CariDetayGoster()
        {
            if (_secilenCari == null) return;

            borderBos.Visibility = Visibility.Collapsed;
            borderDetay.Visibility = Visibility.Visible;

            txtCariAdi.Text = _secilenCari.Musteri;
            txtToplamBorc.Text = $"{_secilenCari.Borc:N2} ₺";
            txtToplamAlacak.Text = $"{_secilenCari.Alacak:N2} ₺";
            txtBakiye.Text = $"{_secilenCari.Bakiye:N2} ₺";

            // Bakiye rengini ayarla
            if (_secilenCari.Bakiye < 0)
            {
                borderBakiye.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F8D7DA"));
                txtBakiye.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#721C24"));
            }

            lstIslemler.ItemsSource = _secilenCari.Islemler.OrderByDescending(i => i.Tarih);
        }

        private void IslemEkle_Click(object sender, RoutedEventArgs e)
        {
            if (_secilenCari == null)
            {
                MessageBox.Show("Lütfen önce bir cari hesap seçin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtIslemTutar.Text) ||
                !decimal.TryParse(txtIslemTutar.Text, out decimal tutar))
            {
                MessageBox.Show("Lütfen geçerli bir tutar girin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var tip = ((ComboBoxItem)cmbIslemTipi.SelectedItem).Tag?.ToString() ?? "tahsilat";
            var aciklama = string.IsNullOrWhiteSpace(txtIslemAciklama.Text)
                ? (tip == "tahsilat" ? "Tahsilat" : "Borç")
                : txtIslemAciklama.Text;

            var yeniIslem = new CariIslem
            {
                Tarih = DateTime.Now,
                Tip = tip,
                Tutar = tutar,
                Aciklama = aciklama
            };

            _secilenCari.Islemler.Add(yeniIslem);

            if (tip == "tahsilat")
            {
                _secilenCari.Alacak += tutar;
            }
            else
            {
                _secilenCari.Borc += tutar;
            }

            var cariHesaplar = _dataService.LoadCariHesaplar();
            var cari = cariHesaplar.FirstOrDefault(c => c.Id == _secilenCari.Id);
            if (cari != null)
            {
                cari.Borc = _secilenCari.Borc;
                cari.Alacak = _secilenCari.Alacak;
                cari.Islemler = _secilenCari.Islemler;
            }
            _dataService.SaveCariHesaplar(cariHesaplar);

            txtIslemTutar.Clear();
            txtIslemAciklama.Clear();

            CariListesiniYukle();
            CariDetayGoster();

            MessageBox.Show("İşlem başarıyla eklendi!", "Başarılı",
                MessageBoxButton.OK, MessageBoxImage.Information);
        private void CariPdfExport_Click(object sender, RoutedEventArgs e)
        {
            if (_secilenCari == null)
            {
                MessageBox.Show("Lütfen bir cari hesap seçin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Dosyası (*.pdf)|*.pdf",
                    FileName = $"CariHesap_{_secilenCari.Musteri.Replace(" ", "_")}.pdf",
                    Title = "PDF Olarak Kaydet"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var pdfService = new PdfExportService();
                    pdfService.CariHesapExport(_secilenCari, saveDialog.FileName);

                    MessageBox.Show("PDF başarıyla oluşturuldu!", "Başarılı",
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
                MessageBox.Show($"PDF oluştururken hata: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CariExcelExport_Click(object sender, RoutedEventArgs e)
        {
            if (_secilenCari == null)
            {
                MessageBox.Show("Lütfen bir cari hesap seçin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Dosyası (*.xlsx)|*.xlsx",
                    FileName = $"CariHesap_{_secilenCari.Musteri.Replace(" ", "_")}.xlsx",
                    Title = "Excel Olarak Kaydet"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var excelService = new ExcelExportService();
                    excelService.CariHesapExport(_secilenCari, saveDialog.FileName);

                    MessageBox.Show("Excel başarıyla oluşturuldu!", "Başarılı",
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
    }
    }
}