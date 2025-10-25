using System;
using System.Collections.Generic;

namespace BucakliogluERP.Models
{
    public class CariHesap
    {
        public int Id { get; set; }
        public string Musteri { get; set; }
        public string Telefon { get; set; }
        public decimal Borc { get; set; }
        public decimal Alacak { get; set; }
        public decimal Bakiye => Borc - Alacak;
        public List<CariIslem> Islemler { get; set; } = new List<CariIslem>();
    }

    public class CariIslem
    {
        public DateTime Tarih { get; set; }
        public string Tip { get; set; } // "borc" veya "tahsilat"
        public decimal Tutar { get; set; }
        public string Aciklama { get; set; }
    }
}