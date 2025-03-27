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
            DataContext = Product;

            var vm = new ProductViewModel();
            CategoryComboBox.ItemsSource = vm.Categories;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
