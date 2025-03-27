using System.Text.RegularExpressions;
using System.Windows;
using SklepInternetowyWPF.Models;
using SklepInternetowyWPF.ViewModels;

namespace SklepInternetowyWPF.Views
{
    public partial class EditProductWindow : Window
    {
        public Product Product { get; }

        public EditProductWindow(Product product)
        {
            InitializeComponent();
            Product = product;

            if (Product.StockMax < Product.Stock)
                Product.StockMax = Product.Stock;

            DataContext = Product;

            var vm = new ProductViewModel();
            CategoryComboBox.ItemsSource = vm.Categories;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Product.Price < 0)
            {
                MessageBox.Show("Cena nie może być ujemna.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Product.Stock < 0)
            {
                MessageBox.Show("Stan magazynowy nie może być ujemny.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Product.Stock % 1 != 0)
            {
                MessageBox.Show("Stan magazynowy musi być liczbą całkowitą.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Product.StockMax = Product.Stock;


            DialogResult = true;
            Close();
        }

        private void StockBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }
    }
}
