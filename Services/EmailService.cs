using System;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Threading.Tasks;

namespace BucakliogluERP.Services
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _kullaniciAdi;
        private readonly string _sifre;

        public EmailService(string smtpServer = "smtp.gmail.com", int smtpPort = 587,
            string kullaniciAdi = "", string sifre = "")
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _kullaniciAdi = kullaniciAdi;
            _sifre = sifre;
        }

        public async Task<bool> SiparisGonder(string aliciEmail, string aliciAdi,
            string pdfDosyaYolu, string siparisNo)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(_kullaniciAdi, "Bucaklıoğlu ERP");
                    mail.To.Add(new MailAddress(aliciEmail, aliciAdi));
                    mail.Subject = $"Sipariş Detayı - {siparisNo}";

                    mail.Body = $@"
                        Sayın {aliciAdi},

                        {siparisNo} numaralı siparişinizin detayları ekte yer almaktadır.

                        İyi günler dileriz.

                        Bucaklıoğlu Saç & Profil & Demir LTD. ŞTİ.
                    ";

                    mail.IsBodyHtml = false;

                    // PDF eki
                    if (File.Exists(pdfDosyaYolu))
                    {
                        var attachment = new Attachment(pdfDosyaYolu);
                        mail.Attachments.Add(attachment);
                    }

                    using (var smtp = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential(_kullaniciAdi, _sifre);
                        await smtp.SendMailAsync(mail);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"E-posta gönderme hatası: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CariEkstreGonder(string aliciEmail, string aliciAdi,
            string pdfDosyaYolu)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(_kullaniciAdi, "Bucaklıoğlu ERP");
                    mail.To.Add(new MailAddress(aliciEmail, aliciAdi));
                    mail.Subject = "Cari Hesap Ekstreniz";

                    mail.Body = $@"
                        Sayın {aliciAdi},

                        Cari hesap ekstreniz ekte yer almaktadır.

                        İyi günler dileriz.

                        Bucaklıoğlu Saç & Profil & Demir LTD. ŞTİ.
                    ";

                    mail.IsBodyHtml = false;

                    if (File.Exists(pdfDosyaYolu))
                    {
                        var attachment = new Attachment(pdfDosyaYolu);
                        mail.Attachments.Add(attachment);
                    }

                    using (var smtp = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential(_kullaniciAdi, _sifre);
                        await smtp.SendMailAsync(mail);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"E-posta gönderme hatası: {ex.Message}");
                return false;
            }
        }
    }
}