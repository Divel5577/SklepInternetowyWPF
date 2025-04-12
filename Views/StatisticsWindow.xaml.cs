using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using SklepInternetowyWPF.ViewModels;

namespace SklepInternetowyWPF.Views
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow(ProductViewModel viewModel)
        {
            InitializeComponent();
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
    }
}
