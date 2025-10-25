using BucakliogluERP.Models;
using BucakliogluERP.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace BucakliogluERP.Views
{
    public partial class AyarlarPage : Page
    {
        private readonly DataService _dataService;
        private readonly string _dataPath;

        public AyarlarPage()
        {
            InitializeComponent();
            _dataService = new DataService();

            _dataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "BucakliogluERP");

            IstatistikleriYukle();
        }

        private void IstatistikleriYukle()
        {
            txtVeriYolu.Text = $"Veri Konumu: {_dataPath}";

            var siparisler = _dataService.LoadSiparisler();
            var cariHesaplar = _dataService.LoadCariHesaplar();

            txtSiparisSayisi.Text = siparisler.Count.ToString();
            txtMusteriSayisi.Text = cariHesaplar.Count.ToString();
            txtUrunSayisi.Text = "18"; // Sabit ürün sayısı
            txtCariSayisi.Text = cariHesaplar.Count.ToString();
        }

        private void YedekAl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Yedek Dosyası (*.zip)|*.zip",
                    FileName = $"BucakliogluERP_Yedek_{DateTime.Now:yyyyMMdd_HHmmss}.zip",
                    Title = "Yedek Dosyasını Kaydet"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    // Tüm JSON dosyalarını kopyala
                    string tempPath = Path.Combine(Path.GetTempPath(), "BucakliogluERP_Temp");
                    if (Directory.Exists(tempPath))
                        Directory.Delete(tempPath, true);

                    Directory.CreateDirectory(tempPath);

                    foreach (var file in Directory.GetFiles(_dataPath, "*.json"))
                    {
                        File.Copy(file, Path.Combine(tempPath, Path.GetFileName(file)));
                    }

                    // ZIP oluştur
                    if (File.Exists(saveDialog.FileName))
                        File.Delete(saveDialog.FileName);

                    System.IO.Compression.ZipFile.CreateFromDirectory(tempPath, saveDialog.FileName);
                    Directory.Delete(tempPath, true);

                    MessageBox.Show("Yedek başarıyla alındı!", "Başarılı",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yedek alınırken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void YedekYukle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openDialog = new OpenFileDialog
                {
                    Filter = "Yedek Dosyası (*.zip)|*.zip",
                    Title = "Yedek Dosyasını Seç"
                };

                if (openDialog.ShowDialog() == true)
                {
                    var result = MessageBox.Show(
                        "Mevcut veriler silinecek! Devam etmek istiyor musunuz?",
                        "Uyarı",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        string tempPath = Path.Combine(Path.GetTempPath(), "BucakliogluERP_Restore");
                        if (Directory.Exists(tempPath))
                            Directory.Delete(tempPath, true);

                        System.IO.Compression.ZipFile.ExtractToDirectory(openDialog.FileName, tempPath);

                        // JSON dosyalarını kopyala
                        foreach (var file in Directory.GetFiles(tempPath, "*.json"))
                        {
                            string destFile = Path.Combine(_dataPath, Path.GetFileName(file));
                            File.Copy(file, destFile, true);
                        }

                        Directory.Delete(tempPath, true);

                        MessageBox.Show("Yedek başarıyla yüklendi! Lütfen uygulamayı yeniden başlatın.",
                            "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);

                        IstatistikleriYukle();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yedek yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VerileriSil_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "TÜM VERİLER SİLİNECEK! Bu işlem geri alınamaz. Emin misiniz?",
                "UYARI",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var result2 = MessageBox.Show(
                    "Son kez soruyoruz: Tüm verileri silmek istediğinize emin misiniz?",
                    "SON UYARI",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Stop);

                if (result2 == MessageBoxResult.Yes)
                {
                    try
                    {
                        foreach (var file in Directory.GetFiles(_dataPath, "*.json"))
                        {
                            File.Delete(file);
                        }

                        MessageBox.Show("Tüm veriler silindi!", "Başarılı",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        IstatistikleriYukle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Veriler silinirken hata oluştu: {ex.Message}", "Hata",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private AppSettings _ayarlar = new AppSettings();

        private void AyarlariYukle()
        {
            _ayarlar = _dataService.LoadData<AppSettings>("settings.json") ?? new AppSettings();

            txtFirmaAdi.Text = _ayarlar.FirmaAdi;
            txtFirmaTelefon.Text = _ayarlar.Telefon;
            txtFirmaEmail.Text = _ayarlar.Email;
            txtFirmaAdres.Text = _ayarlar.Adres;
            txtVergiNo.Text = _ayarlar.VergiNo;

            chkEmailAktif.IsChecked = _ayarlar.EmailGonderimAktif;
            txtSmtpServer.Text = _ayarlar.SmtpServer;
            txtSmtpPort.Text = _ayarlar.SmtpPort.ToString();
            txtSmtpEmail.Text = _ayarlar.SmtpKullaniciAdi;
        }

        private void AyarlariKaydet_Click(object sender, RoutedEventArgs e)
        {
            _ayarlar.FirmaAdi = txtFirmaAdi.Text;
            _ayarlar.Telefon = txtFirmaTelefon.Text;
            _ayarlar.Email = txtFirmaEmail.Text;
            _ayarlar.Adres = txtFirmaAdres.Text;
            _ayarlar.VergiNo = txtVergiNo.Text;

            _ayarlar.EmailGonderimAktif = chkEmailAktif.IsChecked ?? false;
            _ayarlar.SmtpServer = txtSmtpServer.Text;
            if (int.TryParse(txtSmtpPort.Text, out int port))
                _ayarlar.SmtpPort = port;
            _ayarlar.SmtpKullaniciAdi = txtSmtpEmail.Text;

            // Şifre değiştirilmişse kaydet
            if (!string.IsNullOrEmpty(txtSmtpSifre.Password))
                _ayarlar.SmtpSifre = txtSmtpSifre.Password;

            _dataService.SaveData("settings.json", _ayarlar);

            MessageBox.Show("Ayarlar başarıyla kaydedildi!", "Başarılı",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}