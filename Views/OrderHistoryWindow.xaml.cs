using System.Windows;
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
    }
}
