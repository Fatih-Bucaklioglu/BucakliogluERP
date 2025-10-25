using System;
using System.Linq;
using System.Windows.Controls;
using BucakliogluERP.Services;

namespace BucakliogluERP.Views
{
    public partial class MuhasebePage : Page
    {
        private readonly DataService _dataService;

        public MuhasebePage()
        {
            InitializeComponent();
            _dataService = new DataService();
            IstatistikleriYukle();
        }

        private void IstatistikleriYukle()
        {
            var siparisler = _dataService.LoadSiparisler();
            var cariHesaplar = _dataService.LoadCariHesaplar();

            // Toplam Gelir
            decimal toplamGelir = siparisler.Sum(s => s.GenelToplam);
            txtToplamGelir.Text = $"{toplamGelir:N2} ₺";
            txtSiparisSayisi.Text = $"{siparisler.Count} sipariş";

            // Tahsil Edilmemiş
            decimal tahsilEdilmemis = cariHesaplar.Where(c => c.Bakiye < 0).Sum(c => Math.Abs(c.Bakiye));
            txtTahsilEdilmemis.Text = $"{tahsilEdilmemis:N2} ₺";
            txtBorcluMusteri.Text = $"{cariHesaplar.Count(c => c.Bakiye < 0)} müşteri";

            // Bu Ay
            var buAy = DateTime.Now.Month;
            var buYil = DateTime.Now.Year;
            decimal buAySatis = siparisler
                .Where(s => s.Tarih.Month == buAy && s.Tarih.Year == buYil)
                .Sum(s => s.GenelToplam);
            txtBuAy.Text = $"{buAySatis:N2} ₺";

            // Son İşlemler
            var sonIslemler = siparisler
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

            lstSonIslemler.ItemsSource = sonIslemler;
        }
    }
}