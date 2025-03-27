using System.Windows;
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
    }
}
