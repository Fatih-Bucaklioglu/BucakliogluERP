using System.Windows;
using BucakliogluERP.Views;

namespace BucakliogluERP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new SiparisPage());
        }

        private void YeniSiparis_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SiparisPage());
        }

        private void CariHesap_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CariHesapPage());
        }

        private void SiparisGecmisi_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SiparisGecmisiPage());
        }

        private void Musteriler_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MusterilerPage());
        }

        private void Stok_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Stok sayfası yakında eklenecek!");
        }

        private void Muhasebe_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MuhasebePage());
        }

        private void Ayarlar_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AyarlarPage());
        }
    }
}