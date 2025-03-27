using SklepInternetowyWPF.ViewModels;
using System.Windows;
using System.Linq;

namespace SklepInternetowyWPF.Views
{
    public partial class CartWindow : Window
    {
        private CartViewModel cartViewModel;

        public CartWindow(CartViewModel cart)
        {
            InitializeComponent();
            cartViewModel = cart;
            DataContext = cartViewModel;
        }
        private void PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!cartViewModel.CartItems.Any())
            {
                MessageBox.Show("Koszyk jest pusty. Dodaj produkty przed złożeniem zamówienia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            cartViewModel.PlaceOrder();
            MessageBox.Show("Zamówienie złożone!");
            Close();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            cartViewModel.ClearCart();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
