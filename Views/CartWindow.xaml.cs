using SklepInternetowyWPF.ViewModels;
using System.Windows;
using System.Linq;

namespace SklepInternetowyWPF.Views
{
    public partial class CartWindow : Window
    {
        private CartViewModel cartViewModel;
        private readonly ProductViewModel productViewModel;

        public CartWindow(CartViewModel cart, ProductViewModel productViewModel)
        {
            InitializeComponent();
            cartViewModel = cart;
            this.productViewModel = productViewModel;
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
            productViewModel.LoadProducts();
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
