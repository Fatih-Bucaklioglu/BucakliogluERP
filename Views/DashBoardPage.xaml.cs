using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using BucakliogluERP.Services;

namespace BucakliogluERP.Views
{
    public partial class DashboardPage : Page
    {
        private readonly DataService _dataService;
        private readonly DispatcherTimer _timer;

        public DashboardPage()
        {
            InitializeComponent();
            _dataService = new DataService();

            // Tarih/saat güncelleyici
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick; _timer.Start();

            VerileriYukle();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            txtTarihSaat.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        }

        private void VerileriYukle()
        {
            var siparisler = _dataService.LoadSiparisler();
            var cariHesaplar = _dataService.LoadCariHesaplar();

            // Bugünkü siparişler
            var bugunSiparisler = siparisler.Where(s => s.Tarih.Date == DateTime.Now.Date).ToList();
            txtBugunSiparis.Text = bugunSiparisler.Count.ToString();
            txtBugunCiro.Text = $"{bugunSiparisler.Sum(s => s.GenelToplam):N2} ₺";

            // Bekleyen tahsilat
            var bekleyenTahsilat = cariHesaplar.Where(c => c.Bakiye < 0).Sum(c => Math.Abs(c.Bakiye));
            txtBekleyenTahsilat.Text = $"{bekleyenTahsilat:N2} ₺";

            // Aktif müşteriler (son 30 günde sipariş verenler)
            var aktifMusteriler = siparisler
                .Where(s => s.Tarih >= DateTime.Now.AddDays(-30))
                .Select(s => s.Musteri?.Muhattap)
                .Distinct()
                .Count();
            txtAktifMusteri.Text = aktifMusteriler.ToString();

            // Son 10 sipariş
            var sonSiparisler = siparisler
                .OrderByDescending(s => s.Tarih)
                .Take(10)
                .Select(s => new
                {
                    s.SiparisNo,
                    MusteriAdi = s.Musteri?.Muhattap ?? "Bilinmiyor",
                    s.Tarih,
                    s.GenelToplam
                })
                .ToList();

            lstSonSiparisler.ItemsSource = sonSiparisler;
        }

        private void BugunSiparisler_Click(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.NavigateToPage(new SiparisGecmisiPage());
        }

        private void BekleyenTahsilat_Click(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.NavigateToPage(new CariHesapPage());
        }

        private void YeniSiparis_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.NavigateToPage(new SiparisPage());
        }

        private void CariHesap_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.NavigateToPage(new CariHesapPage());
        }

        private void Raporlar_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.NavigateToPage(new RaporlarPage());
        }

        private void Ayarlar_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.NavigateToPage(new AyarlarPage());
        }
    }
}