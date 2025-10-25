using System.Collections.Generic;

namespace BucakliogluERP.Models
{
    public class Urun
    {
        public int Id { get; set; }
        public string Kategori { get; set; }
        public string Ebat { get; set; }
        public decimal Fiyat { get; set; }

        // İskontolu fiyatlar için
        public Dictionary<string, decimal> Fiyatlar { get; set; }
        public bool HasIskonto { get; set; }
    }
}