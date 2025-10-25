using System.Windows;
using BucakliogluERP.Services;

namespace BucakliogluERP.Views
{
    public partial class PrintDialogWindow : Window
    {
        public HtmlPrintService.PrintFormat? SelectedFormat { get; private set; }

        public PrintDialogWindow()
        {
            InitializeComponent();
        }

        private void A4_Click(object sender, RoutedEventArgs e)
        {
            SelectedFormat = HtmlPrintService.PrintFormat.A4;
            DialogResult = true;
            Close();
        }

        private void A5_Click(object sender, RoutedEventArgs e)
        {
            SelectedFormat = HtmlPrintService.PrintFormat.A5;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}