﻿using System.Windows;
using SklepInternetowyWPF.Models;
using SklepInternetowyWPF.ViewModels;
using SklepInternetowyWPF.Views;

namespace SklepInternetowyWPF.Views
{
    public partial class MainWindow : Window
    {
        private ProductViewModel viewModel;
        private readonly CartViewModel cartViewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new ProductViewModel();
            cartViewModel = new CartViewModel(viewModel.CurrentUser?.Username ?? "Gość");
            DataContext = viewModel;

            CategoryFilterBox.ItemsSource = viewModel.Categories;
            CategoryFilterBox.SelectedIndex = 0;

            UpdatePermissionUI();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var product = new Product();
            var window = new EditProductWindow(product);
            if (window.ShowDialog() == true)
            {
                viewModel.SaveProduct(product);
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductList.SelectedItem is Product selected)
            {
                var copy = new Product
                {
                    Id = selected.Id,
                    Name = selected.Name,
                    Description = selected.Description,
                    Price = selected.Price,
                    CategoryId = selected.CategoryId,
                    CategoryName = selected.CategoryName
                };

                var window = new EditProductWindow(copy);
                if (window.ShowDialog() == true)
                {
                    viewModel.SaveProduct(copy);
                }
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductList.SelectedItem is Product selected)
            {
                viewModel.DeleteProduct(selected);
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SearchText = SearchBox.Text.Trim();
            viewModel.SelectedCategoryId = (int)(CategoryFilterBox.SelectedValue ?? 0);
            viewModel.LoadProducts();
        }

        private void SortByName_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.SortBy == "Name")
                viewModel.SortDescending = !viewModel.SortDescending;
            else
            {
                viewModel.SortBy = "Name";
                viewModel.SortDescending = false;
            }

            viewModel.LoadProducts();
        }
        private void ProductList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ProductList.SelectedItem is Product selected)
            {
                var detailsWindow = new ProductDetailsWindow(selected, cartViewModel);
                detailsWindow.ShowDialog();
            }
        }

        private void Cart_Click(object sender, RoutedEventArgs e)
        {
            var window = new CartWindow(cartViewModel);
            window.ShowDialog();
        }
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Product product)
            {
                cartViewModel.AddToCart(product);
            }
        }
        private void History_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(viewModel.CurrentUser?.Username))
            {
                var historyWindow = new OrderHistoryWindow(viewModel.CurrentUser.Username);
                historyWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Zaloguj się, aby zobaczyć historię zamówień.");
            }
        }


        private void SortByPrice_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.SortBy == "Price")
                viewModel.SortDescending = !viewModel.SortDescending;
            else
            {
                viewModel.SortBy = "Price";
                viewModel.SortDescending = false;
            }

            viewModel.LoadProducts();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.CurrentUser == null)
            {
                var login = new LoginWindow();
                if (login.ShowDialog() == true)
                {
                    viewModel.CurrentUser = login.LoggedUser;
                    cartViewModel.CurrentUsername = viewModel.CurrentUser.Username;
                    LoginButton.Content = "Konto";
                    UpdatePermissionUI();
                }
            }
            else
            {
                MessageBox.Show($"Zalogowany jako: {viewModel.CurrentUser.Username}");
            }
        }

        private void UpdatePermissionUI()
        {
            bool isAdmin = viewModel.CurrentUser?.IsAdmin == true;

            AddButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            EditButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            DeleteButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
