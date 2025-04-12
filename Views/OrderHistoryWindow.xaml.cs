using System.Windows;
using System.Windows.Media.Animation;
using SklepInternetowyWPF.ViewModels;

namespace SklepInternetowyWPF.Views
{
    public partial class OrderHistoryWindow : Window
    {
        public OrderHistoryWindow(string username)
        {
            InitializeComponent();
            var vm = new OrderViewModel();
            vm.LoadOrders(username);
            DataContext = vm;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)this.Resources["FadeInStoryboard"];
            storyboard.Begin(this);
        }

    }
}
