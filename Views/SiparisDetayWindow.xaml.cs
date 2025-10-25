using BucakliogluERP.Models;
using BucakliogluERP.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace BucakliogluERP.Views
{
    public partial class SiparisDetayWindow : Window
    {
        private readonly Siparis _siparis;

        public SiparisDetayWindow(Siparis siparis)
        {
            InitializeComponent();
            _siparis = siparis;
            DetaylariGoster();
        }

        private void DetaylariGoster()
        {
            txtSiparisNo.Text = $"Sipariş No: {_siparis.SiparisNo}";
            txtMusteri.Text = $"Müşteri: {_siparis.Musteri?.Muhattap}\n" +
                             $"Telefon: {_siparis.Musteri?.Telefon}\n" +
                             $"Firma: {_siparis.Musteri?.Firma}";
            txtTarih.Text = $"Tarih: {_siparis.Tarih:dd.MM.yyyy}\n" +
                           $"İskonto: {_siparis.IskontoOrani}";

            dgUrunler.ItemsSource = _siparis.Urunler;

            txtAraToplam.Text = $"{_siparis.ToplamTutar:N2} ₺";
            txtKDV.Text = $"{_siparis.KDV:N2} ₺";
            txtGenelToplam.Text = $"{_siparis.GenelToplam:N2} ₺";
        }

        private void PdfExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Dosyası (*.pdf)|*.pdf",
                    FileName = $"Siparis_{_siparis.SiparisNo}.pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var pdfService = new PdfExportService();
                    pdfService.SiparisExport(_siparis, saveDialog.FileName);
                    MessageBox.Show("PDF başarıyla oluşturuldu!", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = saveDialog.FileName, UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void ExcelExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Dosyası (*.xlsx)|*.xlsx",
                    FileName = $"Siparis_{_siparis.SiparisNo}.xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var excelService = new ExcelExportService();
                    excelService.SiparisExport(_siparis, saveDialog.FileName);
                    MessageBox.Show("Excel başarıyla oluşturuldu!", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = saveDialog.FileName, UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Yazdir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDialog = new PrintDialogWindow
                {
                    Owner = this
                };

                if (printDialog.ShowDialog() == true && printDialog.SelectedFormat.HasValue)
                {
                    var htmlService = new HtmlPrintService();
                    string html;

                    if (printDialog.SelectedFormat == HtmlPrintService.PrintFormat.A4)
                    {
                        html = htmlService.GenerateA4Html(_siparis);
                    }
                    else
                    {
                        html = htmlService.GenerateA5Html(_siparis);
                    }

                    string tempPath = Path.Combine(Path.GetTempPath(),
                        $"Siparis_{_siparis.SiparisNo}_{printDialog.SelectedFormat}.html");
                    htmlService.SaveHtmlToFile(html, tempPath);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = tempPath,
                        UseShellExecute = true
                    });

                    MessageBox.Show("Yazdırma penceresi açıldı!", "Bilgi",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Kapat_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}