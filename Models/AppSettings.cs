namespace BucakliogluERP.Models
{
    public class AppSettings
    {
        public string FirmaAdi { get; set; } = "Bucaklıoğlu Saç & Profil & Demir LTD. ŞTİ.";
        public string Adres { get; set; } = "";
        public string Telefon { get; set; } = "";
        public string Email { get; set; } = "";
        public string VergiNo { get; set; } = "";

        // SMTP Ayarları
        public bool EmailGonderimAktif { get; set; } = false;
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string SmtpKullaniciAdi { get; set; } = "";
        public string SmtpSifre { get; set; } = "";

        // Uygulama Ayarları
        public bool OtomatikYedekleme { get; set; } = false;
        public int YedeklemeGunu { get; set; } = 1; // 1=Pazartesi
        public bool KDVDahil { get; set; } = true;
        public decimal KDVOrani { get; set; } = 20m;
        public string VarsayilanIskontoOrani { get; set; } = "35%";

        // Görünüm Ayarları
        public string Tema { get; set; } = "Açık"; // Açık, Koyu
        public double YaziBoyutu { get; set; } = 14;
    }
}