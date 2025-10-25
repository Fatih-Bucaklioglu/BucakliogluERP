using System.Linq;
using System.Windows.Controls;
using BucakliogluERP.Services;

namespace BucakliogluERP.Views
{
    public partial class MusterilerPage : Page
    {
        private readonly DataService _dataService;

        public MusterilerPage()
        {
            InitializeComponent();
            _dataService = new DataService();
            MusteriListesiniYukle();
        }

        private void MusteriListesiniYukle()
        {
            var cariHesaplar = _dataService.LoadCariHesaplar();
            var musteriler = cariHesaplar.Select(c => new
            {
                Muhattap = c.Musteri,
                c.Telefon,
                Firma = c.Telefon // Geçici olarak
            }).ToList();

            lstMusteriler.ItemsSource = musteriler;
        }
    }
}