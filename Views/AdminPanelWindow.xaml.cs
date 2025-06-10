using System.Windows;
using SklepInternetowyWPF.Models;
using SklepInternetowyWPF.ViewModels;
using SklepInternetowyWPF.Data;

namespace SklepInternetowyWPF.Views
{
    public partial class AdminPanelWindow : Window
    {
        private readonly ProductViewModel viewModel;

        public AdminPanelWindow(ProductViewModel vm)
        {
            InitializeComponent();
            viewModel = vm;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var product = new Product();
            var window = new EditProductWindow(product);
            if (window.ShowDialog() == true)
                viewModel.SaveProduct(product);
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            // 1) Otwórz okno wyboru
            var selector = new SelectProductWindow { Owner = this };
            if (selector.ShowDialog() != true) return;

            // 2) Przygotuj kopię do edycji
            var prod = selector.SelectedProduct;
            var tmp = new Product
            {
                Id = prod.Id,
                Name = prod.Name,
                Description = prod.Description,
                Price = prod.Price,
                CategoryId = prod.CategoryId,
                Stock = prod.Stock,
                StockMax = prod.StockMax,
                ImagePath = prod.ImagePath
            };

            // 3) Edycja
            var editWin = new EditProductWindow(tmp) { Owner = this };
            if (editWin.ShowDialog() == true)
                viewModel.SaveProduct(tmp);
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            // 1) Otwórz okno wyboru
            var selector = new SelectProductWindow { Owner = this };
            if (selector.ShowDialog() != true) return;

            // 2) Potwierdzenie i usunięcie
            var prod = selector.SelectedProduct;
            var result = MessageBox.Show(
                $"Czy na pewno chcesz usunąć produkt „{prod.Name}”?",
                "Potwierdź usunięcie",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                viewModel.DeleteProduct(prod);
        }

        private void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            var statsWindow = new StatisticsWindow(viewModel);
            statsWindow.ShowDialog();
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            string name = NewCategoryTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(name))
            {
                viewModel.AddCategory(name);
                viewModel.LoadCategories();
                MessageBox.Show("Dodano kategorię.");
                NewCategoryTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Wprowadź nazwę kategorii.");
            }
        }
    }
}
