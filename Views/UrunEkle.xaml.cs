using System;
using System.Windows;
using System.Windows.Controls;
using BucakliogluERP.Models;

namespace BucakliogluERP.Views
{
    public partial class UrunEkleDialog : Window
    {
        public Stok YeniStok { get; private set; }
        private bool _duzenlemeModu = false;

        public UrunEkleDialog()
        {
            InitializeComponent();
            cmbKategori.SelectedIndex = 0;
        }

        public UrunEkleDialog(Stok stok) : this()
        {
            _duzenlemeModu = true;
            txtBaslik.Text = "Ürün Düzenle";

            // Mevcut değerleri doldur
            txtUrunAdi.Text = stok.UrunAdi;
            txtEbat.Text = stok.Ebat;
            txtMinimumStok.Text = stok.MinimumStok.ToString();
            txtBirimFiyat.Text = stok.BirimFiyat.ToString();

            // Kategori seç
            foreach (ComboBoxItem item in cmbKategori.Items)
            {
                if (item.Tag?.ToString() == stok.Kategori)
                {
                    cmbKategori.SelectedItem = item;
                    break;
                }
            }

            // Birim seç
            foreach (ComboBoxItem item in cmbBirim.Items)
            {
                if (item.Content?.ToString() == stok.Birim)
                {
                    cmbBirim.SelectedItem = item;
                    break;
                }
            }

            YeniStok = stok;
        }

        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
            // Validasyon
            if (cmbKategori.SelectedItem == null)
            {
                MessageBox.Show("Lütfen kategori seçin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUrunAdi.Text))
            {
                MessageBox.Show("Lütfen ürün adı girin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUrunAdi.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEbat.Text))
            {
                MessageBox.Show("Lütfen ebat/özellik girin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEbat.Focus();
                return;
            }

            if (!decimal.TryParse(txtMinimumStok.Text, out decimal minStok) || minStok < 0)
            {
                MessageBox.Show("Lütfen geçerli bir minimum stok değeri girin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMinimumStok.Focus();
                return;
            }

            if (!decimal.TryParse(txtBirimFiyat.Text, out decimal birimFiyat) || birimFiyat <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir birim fiyat girin!", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtBirimFiyat.Focus();
                return;
            }

            // Yeni stok oluştur veya güncelle
            if (!_duzenlemeModu)
            {
                YeniStok = new Stok
                {
                    Id = (int)DateTime.Now.Ticks,
                    MevcutStok = 0,
                    KayitTarihi = DateTime.Now
                };
            }

            YeniStok.Kategori = ((ComboBoxItem)cmbKategori.SelectedItem).Tag?.ToString() ?? "";
            YeniStok.UrunAdi = txtUrunAdi.Text.Trim();
            YeniStok.Ebat = txtEbat.Text.Trim();
            YeniStok.MinimumStok = minStok;
            YeniStok.BirimFiyat = birimFiyat;
            YeniStok.Birim = ((ComboBoxItem)cmbBirim.SelectedItem).Content?.ToString() ?? "Adet";

            DialogResult = true;
            Close();
        }

        private void Iptal_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}