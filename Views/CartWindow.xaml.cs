using SklepInternetowyWPF.ViewModels;
using System.Windows;
using System.Linq;
using System.Windows.Media.Animation;
using SklepInternetowyWPF.Utils;

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
            // 🚫 nie pozwalamy przejść dalej, jeśli koszyk jest pusty
            if (!cartViewModel.CartItems.Any())
            {
                MessageBox.Show(
                    "Koszyk jest pusty. Dodaj przynajmniej jeden produkt przed przejściem do kasy.",
                    "Uwaga",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var checkoutWindow = new CheckoutWindow(cartViewModel);
            checkoutWindow.ShowDialog();

            productViewModel.LoadProducts();
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

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            PdfExporter.ExportCartToPdf(cartViewModel);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)this.Resources["FadeInStoryboard"];
            storyboard.Begin(this);
        }
    }
}
