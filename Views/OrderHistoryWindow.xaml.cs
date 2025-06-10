using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using SklepInternetowyWPF.Utils;
using SklepInternetowyWPF.ViewModels;

namespace SklepInternetowyWPF.Views
{
    public partial class OrderHistoryWindow : Window
    {
        private readonly OrderHistoryViewModel historyViewModel;
        public OrderHistoryWindow(string username)
        {
            InitializeComponent();
            historyViewModel = new OrderHistoryViewModel(username);
            DataContext = historyViewModel;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)this.Resources["FadeInStoryboard"];
            storyboard.Begin(this);
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(OrderContent, "Historia zamówień");
            }
        }
        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            PdfExporter.ExportOrderHistory(historyViewModel);
        }
    }
}
