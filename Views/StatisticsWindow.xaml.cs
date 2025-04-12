using System.Windows;
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

    }
}
