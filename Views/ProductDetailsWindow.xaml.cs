using System.Windows;
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
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Podaj prawidłową ilość.");
                return;
            }

            for (int i = 0; i < quantity; i++)
                cart.AddToCart(product);

            MessageBox.Show($"Dodano {quantity} szt. do koszyka.");
            Close();
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
