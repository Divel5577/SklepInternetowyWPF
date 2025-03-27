using System.Windows;
using System.Windows.Media;
using SklepInternetowyWPF.Models;
using SklepInternetowyWPF.ViewModels;

namespace SklepInternetowyWPF.Views
{
    public partial class ProductDetailsWindow : Window
    {
        private readonly Product product;
        private readonly CartViewModel cart;

        public ProductDetailsWindow(Product product, CartViewModel cart)
        {
            InitializeComponent();
            this.product = product;
            this.cart = cart;

            NameText.Text = product.Name;
            DescText.Text = product.Description;
            PriceText.Text = $"{product.Price:C}";

            SetupStockVisuals();
        }

        private void SetupStockVisuals()
        {
            int maxStock = product.StockMax > 0 ? product.StockMax : 100;
            int stock = product.Stock;
            double percent = maxStock == 0 ? 0 : (double)stock / maxStock;

            StockProgress.Maximum = maxStock;
            StockProgress.Value = stock;

            var fillColor = Colors.Gray;
            if (stock == 0)
                fillColor = Colors.Gray;
            else if (percent < 0.25)
                fillColor = Colors.Red;
            else if (percent < 0.6)
                fillColor = Colors.Orange;
            else
                fillColor = Colors.Green;

            StockProgress.Foreground = new SolidColorBrush(fillColor);

            if (stock == 0)
                StockProgress.ToolTip = "Brak w magazynie.";
            else if (percent < 0.25)
                StockProgress.ToolTip = "Mało produktu – może się szybko wyprzedać.";
            else if (percent < 0.6)
                StockProgress.ToolTip = "Średni poziom zapasów.";
            else
                StockProgress.ToolTip = "Dużo produktu w magazynie.";
        }
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (product.Stock == 0)
            {
                MessageBox.Show("Produkt jest niedostępny. Brak w magazynie.");
                return;
            }

            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Podaj prawidłową ilość.");
                return;
            }

            int added = 0;

            for (int i = 0; i < quantity; i++)
            {
                bool result = cart.AddToCart(product);
                if (!result)
                {
                    MessageBox.Show($"Można dodać maksymalnie {product.Stock} szt. do koszyka.");
                    break;
                }
                added++;
            }

            if (added > 0)
                MessageBox.Show($"Dodano {added} szt. do koszyka.");

            SetupStockVisuals();
            Close();
        }



        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
