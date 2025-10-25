using System;

namespace BucakliogluERP.Models
{
    public class Musteri
    {
        public int Id { get; set; }
        public string Muhattap { get; set; }
        public string Telefon { get; set; }
        public string Firma { get; set; }
        public DateTime KayitTarihi { get; set; }
    }
}