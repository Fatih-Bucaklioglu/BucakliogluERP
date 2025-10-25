namespace BucakliogluERP.Models
{
    public class SiparisKalemi
    {
        public int Id { get; set; }
        public string Kategori { get; set; }
        public string Ebat { get; set; }
        public decimal Miktar { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal Tutar => Miktar * BirimFiyat;
    }
}