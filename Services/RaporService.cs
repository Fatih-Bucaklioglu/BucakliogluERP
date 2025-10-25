using System;
using System.Collections.Generic;
using System.Linq;
using BucakliogluERP.Models;

namespace BucakliogluERP.Services
{
    public class RaporService
    {
        private readonly DataService _dataService;

        public RaporService()
        {
            _dataService = new DataService();
        }

        // Aylık satış raporu
        public List<AylikSatisRaporu> AylikSatisRaporuGetir(int yil)
        {
            var siparisler = _dataService.LoadSiparisler();

            return siparisler
                .Where(s => s.Tarih.Year == yil)
                .GroupBy(s => s.Tarih.Month)
                .Select(g => new AylikSatisRaporu
                {
                    Ay = g.Key,
                    SiparisSayisi = g.Count(),
                    ToplamCiro = g.Sum(s => s.GenelToplam),
                    OrtalamaSiparis = g.Average(s => s.GenelToplam)
                })
                .OrderBy(r => r.Ay)
                .ToList();
        }

        // En çok satan ürünler
        public List<UrunSatisRaporu> EnCokSatanUrunler(int top = 10)
        {
            var siparisler = _dataService.LoadSiparisler();

            return siparisler
                .SelectMany(s => s.Urunler)
                .GroupBy(u => new { u.Kategori, u.Ebat })
                .Select(g => new UrunSatisRaporu
                {
                    Kategori = g.Key.Kategori,
                    Ebat = g.Key.Ebat,
                    ToplamMiktar = g.Sum(u => u.Miktar),
                    ToplamTutar = g.Sum(u => u.Tutar),
                    SatisSayisi = g.Count()
                })
                .OrderByDescending(u => u.ToplamTutar)
                .Take(top)
                .ToList();
        }

        // En karlı müşteriler
        public List<MusteriRaporu> EnKarliMusteriler(int top = 10)
        {
            var siparisler = _dataService.LoadSiparisler();

            return siparisler
                .Where(s => s.Musteri != null)
                .GroupBy(s => s.Musteri!.Muhattap)
                .Select(g => new MusteriRaporu
                {
                    MusteriAdi = g.Key,
                    SiparisSayisi = g.Count(),
                    ToplamHarcama = g.Sum(s => s.GenelToplam),
                    OrtalamaHarcama = g.Average(s => s.GenelToplam),
                    SonSiparisTarihi = g.Max(s => s.Tarih)
                })
                .OrderByDescending(m => m.ToplamHarcama)
                .Take(top)
                .ToList();
        }

        // Kategori bazlı satış raporu
        public List<KategoriRaporu> KategoriSatisRaporu()
        {
            var siparisler = _dataService.LoadSiparisler();

            return siparisler
                .SelectMany(s => s.Urunler)
                .GroupBy(u => u.Kategori)
                .Select(g => new KategoriRaporu
                {
                    Kategori = g.Key,
                    ToplamSatis = g.Sum(u => u.Tutar),
                    ToplamMiktar = g.Sum(u => u.Miktar),
                    UrunSayisi = g.Select(u => u.Ebat).Distinct().Count()
                })
                .OrderByDescending(k => k.ToplamSatis)
                .ToList();
        }

        // Günlük satış trendi
        public List<GunlukSatisRaporu> GunlukSatisTrendi(DateTime baslangic, DateTime bitis)
        {
            var siparisler = _dataService.LoadSiparisler();

            return siparisler
                .Where(s => s.Tarih.Date >= baslangic.Date && s.Tarih.Date <= bitis.Date)
                .GroupBy(s => s.Tarih.Date)
                .Select(g => new GunlukSatisRaporu
                {
                    Tarih = g.Key,
                    SiparisSayisi = g.Count(),
                    ToplamSatis = g.Sum(s => s.GenelToplam)
                })
                .OrderBy(r => r.Tarih)
                .ToList();
        }

        // Ödeme durumu raporu
        public OdemeDurumuRaporu OdemeDurumuRaporuGetir()
        {
            var cariHesaplar = _dataService.LoadCariHesaplar();

            return new OdemeDurumuRaporu
            {
                ToplamBorc = cariHesaplar.Sum(c => c.Borc),
                ToplamAlacak = cariHesaplar.Sum(c => c.Alacak),
                TahsilEdilmemis = cariHesaplar.Where(c => c.Bakiye < 0).Sum(c => Math.Abs(c.Bakiye)),
                BorcluMusteriSayisi = cariHesaplar.Count(c => c.Bakiye < 0),
                AlacakliMusteriSayisi = cariHesaplar.Count(c => c.Bakiye > 0)
            };
        }

        // Performans özeti
        public PerformansOzeti PerformansOzetiGetir(DateTime baslangic, DateTime bitis)
        {
            var siparisler = _dataService.LoadSiparisler()
                .Where(s => s.Tarih >= baslangic && s.Tarih <= bitis)
                .ToList();

            var cariHesaplar = _dataService.LoadCariHesaplar();

            return new PerformansOzeti
            {
                ToplamSiparisSayisi = siparisler.Count,
                ToplamCiro = siparisler.Sum(s => s.GenelToplam),
                OrtalamaSiparisTutari = siparisler.Any() ? siparisler.Average(s => s.GenelToplam) : 0,
                ToplamMusteriSayisi = siparisler.Select(s => s.Musteri?.Muhattap).Distinct().Count(),
                TahsilEdilmemis = cariHesaplar.Where(c => c.Bakiye < 0).Sum(c => Math.Abs(c.Bakiye)),
                EnBuyukSiparis = siparisler.Any() ? siparisler.Max(s => s.GenelToplam) : 0,
                EnKucukSiparis = siparisler.Any() ? siparisler.Min(s => s.GenelToplam) : 0
            };
        }
    }

    // Rapor Model Sınıfları
    public class AylikSatisRaporu
    {
        public int Ay { get; set; }
        public string AyAdi => new DateTime(2000, Ay, 1).ToString("MMMM");
        public int SiparisSayisi { get; set; }
        public decimal ToplamCiro { get; set; }
        public decimal OrtalamaSiparis { get; set; }
    }

    public class UrunSatisRaporu
    {
        public string Kategori { get; set; } = string.Empty;
        public string Ebat { get; set; } = string.Empty;
        public decimal ToplamMiktar { get; set; }
        public decimal ToplamTutar { get; set; }
        public int SatisSayisi { get; set; }
    }

    public class MusteriRaporu
    {
        public string MusteriAdi { get; set; } = string.Empty;
        public int SiparisSayisi { get; set; }
        public decimal ToplamHarcama { get; set; }
        public decimal OrtalamaHarcama { get; set; }
        public DateTime SonSiparisTarihi { get; set; }
    }

    public class KategoriRaporu
    {
        public string Kategori { get; set; } = string.Empty;
        public decimal ToplamSatis { get; set; }
        public decimal ToplamMiktar { get; set; }
        public int UrunSayisi { get; set; }
    }

    public class GunlukSatisRaporu
    {
        public DateTime Tarih { get; set; }
        public int SiparisSayisi { get; set; }
        public decimal ToplamSatis { get; set; }
    }

    public class OdemeDurumuRaporu
    {
        public decimal ToplamBorc { get; set; }
        public decimal ToplamAlacak { get; set; }
        public decimal TahsilEdilmemis { get; set; }
        public int BorcluMusteriSayisi { get; set; }
        public int AlacakliMusteriSayisi { get; set; }
    }

    public class PerformansOzeti
    {
        public int ToplamSiparisSayisi { get; set; }
        public decimal ToplamCiro { get; set; }
        public decimal OrtalamaSiparisTutari { get; set; }
        public int ToplamMusteriSayisi { get; set; }
        public decimal TahsilEdilmemis { get; set; }
        public decimal EnBuyukSiparis { get; set; }
        public decimal EnKucukSiparis { get; set; }
    }
}