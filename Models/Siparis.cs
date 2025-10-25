using System;
using System.Collections.Generic;
using System.Linq;

namespace BucakliogluERP.Models
{
    public class Siparis
    {
        public int Id { get; set; }
        public string SiparisNo { get; set; }
        public DateTime Tarih { get; set; }
        public Musteri Musteri { get; set; }
        public string IskontoOrani { get; set; }
        public List<SiparisKalemi> Urunler { get; set; } = new List<SiparisKalemi>();

        public decimal ToplamTutar => Urunler.Sum(u => u.Tutar);
        public decimal KDV => ToplamTutar * 0.2m;
        public decimal GenelToplam => ToplamTutar + KDV;
    }
}