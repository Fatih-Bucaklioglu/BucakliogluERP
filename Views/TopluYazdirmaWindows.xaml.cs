using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using BucakliogluERP.Models;
using BucakliogluERP.Services;

namespace BucakliogluERP.Views
{
    public partial class TopluYazdirmaWindow : Window
    {
        private readonly ObservableCollection<SiparisSecim> _siparisler;
        private readonly HtmlPrintService _htmlService;

        public TopluYazdirmaWindow()
        {
            InitializeComponent();
            _siparisler = new ObservableCollection<SiparisSecim>();
            _htmlService = new HtmlPrintService();

            SiparisleriYukle();
            lstSiparisler.ItemsSource = _siparisler;

            // Seçim değişikliklerini dinle
            _siparisler.CollectionChanged += (s, e) => SecilenSayisiniGuncelle();
        }

        private void SiparisleriYukle()
        {
            var dataService = new DataService();
            var siparisler = dataService.LoadSiparisler()
                .OrderByDescending(s => s.Tarih)
                .Take(50) // Son 50 sipariş
                .ToList();

            foreach (var siparis in siparisler)
            {
                _siparisler.Add(new SiparisSecim { Siparis = siparis, Secili = false });
            }
        }

        private void TumunuSec_Changed(object sender, RoutedEventArgs e)
        {
            bool secili = chkTumunuSec.IsChecked ?? false;
            foreach (var item in _siparisler)
            {
                item.Secili = secili;
            }
            lstSiparisler.Items.Refresh();
            SecilenSayisiniGuncelle();
        }

        private void SecilenSayisiniGuncelle()
        {
            int secilenSayisi = _siparisler.Count(s => s.Secili);
            txtSecilenSayisi.Text = $"{secilenSayisi} sipariş seçildi";
        }

        private void A4Yazdir_Click(object sender, RoutedEventArgs e)
        {
            TopluYazdir(HtmlPrintService.PrintFormat.A4);
        }

        private void A5Yazdir_Click(object sender, RoutedEventArgs e)
        {
            TopluYazdir(HtmlPrintService.PrintFormat.A5);
        }

        private void TopluYazdir(HtmlPrintService.PrintFormat format)
        {
            var secilenler = _siparisler.Where(s => s.Secili).ToList();

            if (secilenler.Count == 0)
            {
                MessageBox.Show("Lütfen en az bir sipariş seçin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string tempFolder = Path.Combine(Path.GetTempPath(), $"TopluYazdir_{DateTime.Now:yyyyMMddHHmmss}");
                Directory.CreateDirectory(tempFolder);

                foreach (var secim in secilenler)
                {
                    string html = format == HtmlPrintService.PrintFormat.A4
                        ? _htmlService.GenerateA4Html(secim.Siparis)
                        : _htmlService.GenerateA5Html(secim.Siparis);

                    string fileName = $"{secim.Siparis.SiparisNo}_{format}.html";
                    string filePath = Path.Combine(tempFolder, fileName);
                    _htmlService.SaveHtmlToFile(html, filePath);
                }

                // Klasörü aç
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempFolder,
                    UseShellExecute = true
                });

                MessageBox.Show($"{secilenler.Count} sipariş HTML olarak oluşturuldu!\n\nDosyaları tarayıcıda açıp yazdırabilirsiniz.",
                    "Başarılı",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Toplu yazdırma hatası: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Iptal_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    // Yardımcı sınıf
    public class SiparisSecim : System.ComponentModel.INotifyPropertyChanged
    {
        private bool _secili;

        public Siparis Siparis { get; set; } = new();

        public bool Secili
        {
            get => _secili;
            set
            {
                _secili = value;
                OnPropertyChanged(nameof(Secili));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}