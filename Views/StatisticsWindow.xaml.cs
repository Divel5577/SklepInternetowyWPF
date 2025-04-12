using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using SklepInternetowyWPF.Utils;
using SklepInternetowyWPF.ViewModels;

namespace SklepInternetowyWPF.Views
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow(ProductViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            StatsListView.ItemsSource = viewModel.GetTopSellingProducts();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)this.Resources["FadeInStoryboard"];
            storyboard.Begin(this);
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                dialog.PrintVisual(StatsListView, "Statystyki sprzedaży");
            }
        }
        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProductViewModel vm)
                PdfExporter.ExportStatistics(vm);
        }

    }
}
