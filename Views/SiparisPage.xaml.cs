using BucakliogluERP.Helpers;
using BucakliogluERP.Models;
using BucakliogluERP.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BucakliogluERP.Views
{
    public partial class SiparisPage : Page
    {
        private DataService _dataService;
        private List<Urun> _fiyatListesi;
        private ObservableCollection<SiparisKalemiDisplay> _siparisKalemleri;
        private string _secilenKategori = "";
        private Button _secilenKategoriButonu = null;

        public SiparisPage()
        {
            InitializeComponent();
            _dataService = new DataService();
            _siparisKalemleri = new ObservableCollection<SiparisKalemiDisplay>();
            dgSiparisler.ItemsSource = _siparisKalemleri;

            // Varsayılan fiyat listesini yükle
            FiyatListesiYukle();

            // Sipariş numarası otomatik oluştur
            txtSiparisNo.Text = $"SIP-{DateTime.Now:yyyyMMddHHmmss}";
        }
        private void Onizleme_Click(object sender, RoutedEventArgs e)
        {
            if (_siparisKalemleri.Count == 0)
            {
                ToastNotification.Warning("Önce sipariş oluşturun!");
                return;
            }

            try
            {
                var printDialog = new PrintDialogWindow
                {
                    Owner = Window.GetWindow(this)
                };

                if (printDialog.ShowDialog() == true && printDialog.SelectedFormat.HasValue)
                {
                    var siparis = new Siparis
                    {
                        SiparisNo = txtSiparisNo.Text,
                        Tarih = dpTarih.SelectedDate ?? DateTime.Now,
                        Musteri = new Musteri
                        {
                            Muhattap = txtMusteriAdi.Text,
                            Telefon = txtTelefon.Text,
                            Firma = txtFirma.Text
                        },
                        IskontoOrani = panelIskonto.Visibility == Visibility.Visible && cmbIskonto.SelectedItem is ComboBoxItem item
                            ? item.Tag?.ToString() ?? ""
                            : "",
                        Urunler = _siparisKalemleri.Select(k => new SiparisKalemi
                        {
                            Kategori = k.Kategori,
                            Ebat = k.Ebat,
                            Miktar = k.Miktar,
                            BirimFiyat = k.BirimFiyat
                        }).ToList()
                    };

                    var htmlService = new HtmlPrintService();
                    string html = printDialog.SelectedFormat == HtmlPrintService.PrintFormat.A4
                        ? htmlService.CreateA4Html(siparis)
                        : htmlService.CreateA5Html(siparis);

                    var previewWindow = new HtmlPreviewWindow(html)
                    {
                        Owner = Window.GetWindow(this)
                    };
                    previewWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ToastNotification.Error($"Önizleme hatası: {ex.Message}");
            }
        }

        private void FiyatListesiYukle()
        {
            _fiyatListesi = new List<Urun>
            {
                // Profil kategorisi
                new Urun {
                    Kategori = "profil",
                    Ebat = "10x10-1",
                    HasIskonto = true,
                    Fiyatlar = new Dictionary<string, decimal> {
                        {"28%", 102}, {"29%", 101}, {"30%", 99}, {"31%", 98},
                        {"32%", 96}, {"33%", 95}, {"34%", 94}, {"35%", 92},
                        {"36%", 91}, {"37%", 89}, {"38%", 88}, {"39%", 86}
                    }
                },
                new Urun {
                    Kategori = "profil",
                    Ebat = "10x10-1,20",
                    HasIskonto = true,
                    Fiyatlar = new Dictionary<string, decimal> {
                        {"28%", 122}, {"29%", 120}, {"30%", 118}, {"31%", 117},
                        {"32%", 115}, {"33%", 113}, {"34%", 111}, {"35%", 110},
                        {"36%", 108}, {"37%", 106}, {"38%", 105}, {"39%", 103}
                    }
                },
                new Urun {
                    Kategori = "profil",
                    Ebat = "15x15-1,50",
                    HasIskonto = true,
                    Fiyatlar = new Dictionary<string, decimal> {
                        {"28%", 179}, {"29%", 176}, {"30%", 174}, {"31%", 171},
                        {"32%", 169}, {"33%", 166}, {"34%", 164}, {"35%", 161},
                        {"36%", 159}, {"37%", 156}, {"38%", 154}, {"39%", 152}
                    }
                },
                new Urun {
                    Kategori = "profil",
                    Ebat = "20x20-1",
                    HasIskonto = true,
                    Fiyatlar = new Dictionary<string, decimal> {
                        {"28%", 158}, {"29%", 156}, {"30%", 154}, {"31%", 152},
                        {"32%", 150}, {"33%", 147}, {"34%", 145}, {"35%", 143},
                        {"36%", 141}, {"37%", 139}, {"38%", 136}, {"39%", 134}
                    }
                },
                new Urun {
                    Kategori = "profil",
                    Ebat = "25x25-1,20",
                    HasIskonto = true,
                    Fiyatlar = new Dictionary<string, decimal> {
                        {"28%", 234}, {"29%", 230}, {"30%", 227}, {"31%", 224},
                        {"32%", 221}, {"33%", 217}, {"34%", 214}, {"35%", 211},
                        {"36%", 208}, {"37%", 204}, {"38%", 201}, {"39%", 198}
                    }
                },
                new Urun {
                    Kategori = "profil",
                    Ebat = "30x30-1,40",
                    HasIskonto = true,
                    Fiyatlar = new Dictionary<string, decimal> {
                        {"28%", 296}, {"29%", 292}, {"30%", 288}, {"31%", 284},
                        {"32%", 280}, {"33%", 276}, {"34%", 272}, {"35%", 268},
                        {"36%", 264}, {"37%", 259}, {"38%", 255}, {"39%", 251}
                    }
                },
                new Urun {
                    Kategori = "profil",
                    Ebat = "40x40-2",
                    HasIskonto = true,
                    Fiyatlar = new Dictionary<string, decimal> {
                        {"28%", 378}, {"29%", 373}, {"30%", 368}, {"31%", 362},
                        {"32%", 357}, {"33%", 352}, {"34%", 347}, {"35%", 341},
                        {"36%", 336}, {"37%", 331}, {"38%", 326}, {"39%", 320}
                    }
                },
                
                // Boru kategorisi
                new Urun {
                    Kategori = "boru",
                    Ebat = "20x30-1,20",
                    HasIskonto = true,
                    Fiyatlar = new Dictionary<string, decimal> {
                        {"28%", 234}, {"29%", 230}, {"30%", 227}, {"31%", 224},
                        {"32%", 221}, {"33%", 217}, {"34%", 214}, {"35%", 211},
                        {"36%", 208}, {"37%", 204}, {"38%", 201}, {"39%", 198}
                    }
                },
                new Urun {
                    Kategori = "boru",
                    Ebat = "20x40-1,50",
                    HasIskonto = true,
                    Fiyatlar = new Dictionary<string, decimal> {
                        {"28%", 307}, {"29%", 303}, {"30%", 299}, {"31%", 295},
                        {"32%", 290}, {"33%", 286}, {"34%", 282}, {"35%", 277},
                        {"36%", 273}, {"37%", 269}, {"38%", 265}, {"39%", 260}
                    }
                },
                
                // Saç kategorisi (iskontosuz)
                new Urun {
                    Kategori = "sac",
                    Ebat = "DKP Sac 1mm",
                    HasIskonto = false,
                    Fiyat = 450
                },
                new Urun {
                    Kategori = "sac",
                    Ebat = "DKP Sac 2mm",
                    HasIskonto = false,
                    Fiyat = 520
                },
                new Urun {
                    Kategori = "sac",
                    Ebat = "Galvaniz Sac 0.5mm",
                    HasIskonto = false,
                    Fiyat = 380
                },
                
                // Hırdavat kategorisi (iskontosuz)
                new Urun {
                    Kategori = "hirdavat",
                    Ebat = "Yaylı Sürgü 15cm",
                    HasIskonto = false,
                    Fiyat = 25
                },
                new Urun {
                    Kategori = "hirdavat",
                    Ebat = "Yaylı Sürgü 20cm",
                    HasIskonto = false,
                    Fiyat = 35
                },
                new Urun {
                    Kategori = "hirdavat",
                    Ebat = "Menteşe Büyük",
                    HasIskonto = false,
                    Fiyat = 18
                },
                new Urun {
                    Kategori = "hirdavat",
                    Ebat = "Menteşe Küçük",
                    HasIskonto = false,
                    Fiyat = 12
                },
                
                // Lema kategorisi (iskontosuz)
                new Urun {
                    Kategori = "lema",
                    Ebat = "Lema Delici 1mm",
                    HasIskonto = false,
                    Fiyat = 85
                },
                new Urun {
                    Kategori = "lema",
                    Ebat = "Lema Delici 2mm",
                    HasIskonto = false,
                    Fiyat = 95
                }
            };
        }

        private void Kategori_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            _secilenKategori = btn.Tag.ToString();

            // Önceki butonu normal renge çevir
            if (_secilenKategoriButonu != null)
            {
                _secilenKategoriButonu.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECF0F1"));
                _secilenKategoriButonu.Foreground = Brushes.Black;
            }

            // Yeni butonu vurgula
            btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#14A085"));
            btn.Foreground = Brushes.White;
            _secilenKategoriButonu = btn;

            // Ürün listesini güncelle
            UrunListesiGuncelle();

            // Ürün ekleme bölümünü göster
            borderUrunEkleme.Visibility = Visibility.Visible;
        }

        private void UrunListesiGuncelle()
        {
            cmbUrunler.Items.Clear();

            var urunler = _fiyatListesi.Where(u => u.Kategori == _secilenKategori).ToList();

            foreach (var urun in urunler)
            {
                cmbUrunler.Items.Add(urun.Ebat);
            }

            // İskonto panelini göster/gizle
            if (urunler.Any() && urunler[0].HasIskonto)
            {
                panelIskonto.Visibility = Visibility.Visible;
            }
            else
            {
                panelIskonto.Visibility = Visibility.Collapsed;
            }
        }

        private void cmbUrunler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ürün seçildiğinde bir şey yapabiliriz
        }

        private void cmbIskonto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // İskonto değiştiğinde mevcut siparişleri güncelle
            if (cmbIskonto.SelectedItem == null) return;

            string yeniIskonto = ((ComboBoxItem)cmbIskonto.SelectedItem).Tag.ToString();

            foreach (var kalem in _siparisKalemleri)
            {
                if (kalem.HasIskonto)
                {
                    var urun = _fiyatListesi.FirstOrDefault(u =>
                        u.Ebat == kalem.Ebat && u.Kategori == kalem.Kategori);

                    if (urun != null && urun.Fiyatlar.ContainsKey(yeniIskonto))
                    {
                        kalem.BirimFiyat = urun.Fiyatlar[yeniIskonto];
                        kalem.Tutar = kalem.Miktar * kalem.BirimFiyat;
                    }
                }
            }

            dgSiparisler.Items.Refresh();
            ToplamilariHesapla();
        }

        private void UrunEkle_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_secilenKategori) || cmbUrunler.SelectedItem == null)
            {
                MessageBox.Show("Lütfen kategori ve ürün seçin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string secilenEbat = cmbUrunler.SelectedItem.ToString();
            var urun = _fiyatListesi.FirstOrDefault(u =>
                u.Ebat == secilenEbat && u.Kategori == _secilenKategori);

            if (urun == null) return;

            decimal fiyat;
            if (urun.HasIskonto)
            {
                string iskonto = ((ComboBoxItem)cmbIskonto.SelectedItem).Tag.ToString();
                fiyat = urun.Fiyatlar[iskonto];
            }
            else
            {
                fiyat = urun.Fiyat;
            }

            var yeniKalem = new SiparisKalemiDisplay
            {
                No = _siparisKalemleri.Count + 1,
                Kategori = KategoriAdiniAl(_secilenKategori),
                Ebat = urun.Ebat,
                Miktar = 0,
                BirimFiyat = fiyat,
                Tutar = 0,
                HasIskonto = urun.HasIskonto
            };

            _siparisKalemleri.Add(yeniKalem);

            // Tabloyu göster
            borderSiparisTablosu.Visibility = Visibility.Visible;

            ToplamilariHesapla();
        }

        private string KategoriAdiniAl(string kategori)
        {
            return kategori switch
            {
                "profil" => "Profil",
                "boru" => "Boru",
                "sac" => "Saç",
                "hirdavat" => "Hırdavat",
                "lema" => "Lema",
                _ => kategori
            };
        }

        private void SiparisKalemiSil_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            SiparisKalemiDisplay kalem = btn.DataContext as SiparisKalemiDisplay;

            if (kalem != null)
            {
                _siparisKalemleri.Remove(kalem);

                // Numaraları yeniden düzenle
                for (int i = 0; i < _siparisKalemleri.Count; i++)
                {
                    _siparisKalemleri[i].No = i + 1;
                }

                dgSiparisler.Items.Refresh();
                ToplamilariHesapla();

                // Liste boşsa tabloyu gizle
                if (_siparisKalemleri.Count == 0)
                {
                    borderSiparisTablosu.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ToplamilariHesapla()
        {
            decimal araToplam = _siparisKalemleri.Sum(k => k.Tutar);
            decimal kdv = araToplam * 0.2m;
            decimal genelToplam = araToplam + kdv;

            txtAraToplam.Text = $"{araToplam:N2} ₺";
            txtKDV.Text = $"{kdv:N2} ₺";
            txtGenelToplam.Text = $"{genelToplam:N2} ₺";
        }

        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
            if (_siparisKalemleri.Count == 0)
            {
                MessageBox.Show("Lütfen en az bir ürün ekleyin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMusteriAdi.Text))
            {
                MessageBox.Show("Lütfen müşteri adı girin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Sipariş oluştur
                var siparis = new Siparis
                {
                    Id = (int)DateTime.Now.Ticks,
                    SiparisNo = txtSiparisNo.Text,
                    Tarih = dpTarih.SelectedDate ?? DateTime.Now,
                    Musteri = new Musteri
                    {
                        Muhattap = txtMusteriAdi.Text,
                        Telefon = txtTelefon.Text,
                        Firma = txtFirma.Text
                    },
                    IskontoOrani = panelIskonto.Visibility == Visibility.Visible
                        ? ((ComboBoxItem)cmbIskonto.SelectedItem).Tag.ToString()
                        : "",
                    Urunler = _siparisKalemleri.Select(k => new SiparisKalemi
                    {
                        Kategori = k.Kategori,
                        Ebat = k.Ebat,
                        Miktar = k.Miktar,
                        BirimFiyat = k.BirimFiyat
                    }).ToList()
                };

                // Siparişleri yükle
                var siparisler = _dataService.LoadSiparisler();
                siparisler.Insert(0, siparis);
                _dataService.SaveSiparisler(siparisler);

                // Cari hesaba ekle
                var cariHesaplar = _dataService.LoadCariHesaplar();
                var cari = cariHesaplar.FirstOrDefault(c => c.Musteri == txtMusteriAdi.Text);

                if (cari == null)
                {
                    cari = new CariHesap
                    {
                        Id = (int)DateTime.Now.Ticks,
                        Musteri = txtMusteriAdi.Text,
                        Telefon = txtTelefon.Text,
                        Borc = siparis.GenelToplam,
                        Alacak = 0,
                        Islemler = new List<CariIslem>
                        {
                            new CariIslem
                            {
                                Tarih = DateTime.Now,
                                Tip = "borc",
                                Tutar = siparis.GenelToplam,
                                Aciklama = $"Sipariş: {siparis.SiparisNo}"
                            }
                        }
                    };
                    cariHesaplar.Add(cari);
                }
                else
                {
                    cari.Borc += siparis.GenelToplam;
                    cari.Islemler.Add(new CariIslem
                    {
                        Tarih = DateTime.Now,
                        Tip = "borc",
                        Tutar = siparis.GenelToplam,
                        Aciklama = $"Sipariş: {siparis.SiparisNo}"
                    });
                }

                _dataService.SaveCariHesaplar(cariHesaplar);

                MessageBox.Show("Sipariş başarıyla kaydedildi!", "Başarılı",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Formu temizle
                FormuTemizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sipariş kaydedilirken hata oluştu: {ex.Message}",
                    "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FormuTemizle()
        {
            txtMusteriAdi.Clear();
            txtTelefon.Clear();
            txtFirma.Clear();
            txtSiparisNo.Text = $"SIP-{DateTime.Now:yyyyMMddHHmmss}";
            dpTarih.SelectedDate = DateTime.Now;

            _siparisKalemleri.Clear();
            borderSiparisTablosu.Visibility = Visibility.Collapsed;
            borderUrunEkleme.Visibility = Visibility.Collapsed;

            if (_secilenKategoriButonu != null)
            {
                _secilenKategoriButonu.Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#ECF0F1"));
                _secilenKategoriButonu.Foreground = Brushes.Black;
            }

            _secilenKategori = "";
            ToplamilariHesapla();
        }

        // SiparisPage.xaml.cs içinde ExportPDF_Click ve Yazdir_Click metodlarını güncelleyin

        private void ExportPDF_Click(object sender, RoutedEventArgs e)
        {
            if (_siparisKalemleri.Count == 0)
            {
                MessageBox.Show("Önce sipariş oluşturun!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Dosyası (*.pdf)|*.pdf",
                    FileName = $"Siparis_{txtSiparisNo.Text}.pdf",
                    Title = "PDF Olarak Kaydet"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var siparis = new Siparis
                    {
                        SiparisNo = txtSiparisNo.Text,
                        Tarih = dpTarih.SelectedDate ?? DateTime.Now,
                        Musteri = new Musteri
                        {
                            Muhattap = txtMusteriAdi.Text,
                            Telefon = txtTelefon.Text,
                            Firma = txtFirma.Text
                        },
                        IskontoOrani = panelIskonto.Visibility == Visibility.Visible && cmbIskonto.SelectedItem is ComboBoxItem item
                            ? item.Tag?.ToString() ?? ""
                            : "",
                        Urunler = _siparisKalemleri.Select(k => new SiparisKalemi
                        {
                            Kategori = k.Kategori,
                            Ebat = k.Ebat,
                            Miktar = k.Miktar,
                            BirimFiyat = k.BirimFiyat
                        }).ToList()
                    };

                    var pdfService = new PdfExportService();
                    pdfService.SiparisExport(siparis, saveDialog.FileName);

                    MessageBox.Show("PDF başarıyla oluşturuldu!", "Başarılı",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // PDF'i aç
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveDialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PDF oluştururken hata: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_siparisKalemleri.Count == 0)
            {
                MessageBox.Show("Önce sipariş oluşturun!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Dosyası (*.xlsx)|*.xlsx",
                    FileName = $"Siparis_{txtSiparisNo.Text}.xlsx",
                    Title = "Excel Olarak Kaydet"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var siparis = new Siparis
                    {
                        SiparisNo = txtSiparisNo.Text,
                        Tarih = dpTarih.SelectedDate ?? DateTime.Now,
                        Musteri = new Musteri
                        {
                            Muhattap = txtMusteriAdi.Text,
                            Telefon = txtTelefon.Text,
                            Firma = txtFirma.Text
                        },
                        IskontoOrani = panelIskonto.Visibility == Visibility.Visible && cmbIskonto.SelectedItem is ComboBoxItem item
                            ? item.Tag?.ToString() ?? ""
                            : "",
                        Urunler = _siparisKalemleri.Select(k => new SiparisKalemi
                        {
                            Kategori = k.Kategori,
                            Ebat = k.Ebat,
                            Miktar = k.Miktar,
                            BirimFiyat = k.BirimFiyat
                        }).ToList()
                    };

                    var excelService = new ExcelExportService();
                    excelService.SiparisExport(siparis, saveDialog.FileName);

                    MessageBox.Show("Excel başarıyla oluşturuldu!", "Başarılı",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Excel'i aç
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveDialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excel oluştururken hata: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Yazdir_Click(object sender, RoutedEventArgs e)
        {
            if (_siparisKalemleri.Count == 0)
            {
                MessageBox.Show("Önce sipariş oluşturun!");
                return;
            }

            try
            {
                var printDialog = new PrintDialogWindow { Owner = Window.GetWindow(this) };

                if (printDialog.ShowDialog() == true && printDialog.SelectedFormat.HasValue)
                {
                    var siparis = new Siparis
                    {
                        SiparisNo = txtSiparisNo.Text,
                        Tarih = dpTarih.SelectedDate ?? DateTime.Now,
                        Musteri = new Musteri
                        {
                            Muhattap = txtMusteriAdi.Text,
                            Telefon = txtTelefon.Text,
                            Firma = txtFirma.Text
                        },
                        IskontoOrani = panelIskonto.Visibility == Visibility.Visible && cmbIskonto.SelectedItem is ComboBoxItem item
                            ? item.Tag?.ToString() ?? ""
                            : "",
                        Urunler = _siparisKalemleri.Select(k => new SiparisKalemi
                        {
                            Kategori = k.Kategori,
                            Ebat = k.Ebat,
                            Miktar = k.Miktar,
                            BirimFiyat = k.BirimFiyat
                        }).ToList()
                    };

                    var htmlService = new HtmlPrintService();
                    string html = printDialog.SelectedFormat == HtmlPrintService.PrintFormat.A4
                        ? htmlService.CreateA4Html(siparis)
                        : htmlService.CreateA5Html(siparis);

                    string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                        $"Siparis_{txtSiparisNo.Text}.html");
                    htmlService.SaveToFile(html, tempPath);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = tempPath,
                        UseShellExecute = true
                    });

                    MessageBox.Show("Tarayıcıda açıldı! Ctrl+P ile yazdırabilirsiniz.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }
    }

    // DataGrid için yardımcı sınıf
    public class SiparisKalemiDisplay : System.ComponentModel.INotifyPropertyChanged
    {
        private int _no;
        private string _kategori;
        private string _ebat;
        private decimal _miktar;
        private decimal _birimFiyat;
        private decimal _tutar;

        public int No
        {
            get => _no;
            set { _no = value; OnPropertyChanged(nameof(No)); }
        }

        public string Kategori
        {
            get => _kategori;
            set { _kategori = value; OnPropertyChanged(nameof(Kategori)); }
        }

        public string Ebat
        {
            get => _ebat;
            set { _ebat = value; OnPropertyChanged(nameof(Ebat)); }
        }

        public decimal Miktar
        {
            get => _miktar;
            set
            {
                _miktar = value;
                Tutar = _miktar * _birimFiyat;
                OnPropertyChanged(nameof(Miktar));
            }
        }

        public decimal BirimFiyat
        {
            get => _birimFiyat;
            set
            {
                _birimFiyat = value;
                Tutar = _miktar * _birimFiyat;
                OnPropertyChanged(nameof(BirimFiyat));
            }
        }

        public decimal Tutar
        {
            get => _tutar;
            set { _tutar = value; OnPropertyChanged(nameof(Tutar)); }
        }

        public bool HasIskonto { get; set; }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}