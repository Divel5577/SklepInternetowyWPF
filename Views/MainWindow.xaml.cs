using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using SklepInternetowyWPF.Data;
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

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductList.SelectedItem is Product selected)
            {
                using (var db = new AppDbContext())
                {
                    var productFromDb = db.Products.FirstOrDefault(p => p.Id == selected.Id);
                    if (productFromDb == null)
                        return;

                    var tempProduct = new Product
                    {
                        Id = productFromDb.Id,
                        Name = productFromDb.Name,
                        Description = productFromDb.Description,
                        Price = productFromDb.Price,
                        CategoryId = productFromDb.CategoryId,
                        Stock = productFromDb.Stock,
                        StockMax = productFromDb.StockMax,
                        ImagePath = productFromDb.ImagePath
                    };

                    var window = new EditProductWindow(tempProduct);
                    if (window.ShowDialog() == true)
                    {
                        productFromDb.Name = tempProduct.Name;
                        productFromDb.Description = tempProduct.Description;
                        productFromDb.Price = tempProduct.Price;
                        productFromDb.CategoryId = tempProduct.CategoryId;
                        productFromDb.Stock = tempProduct.Stock;
                        productFromDb.StockMax = tempProduct.StockMax;
                        productFromDb.ImagePath = tempProduct.ImagePath;

                        db.SaveChanges();

                        viewModel.LoadProducts(); 
                    }
                }
            }
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim();

            viewModel.SearchText = query;
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
            var window = new CartWindow(cartViewModel, viewModel);
            window.ShowDialog();
            viewModel.LoadProducts();
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
                // 1) Pokaż okno logowania
                var login = new LoginWindow();
                if (login.ShowDialog() == true)
                {
                    // 2) Ustawiamy aktualnie zalogowanego
                    viewModel.CurrentUser = login.LoggedUser;
                    cartViewModel.CurrentUsername = login.LoggedUser.Username;

                    // 3) Zmieniamy tekst przycisku na "Konto"
                    LoginButton.Content = "Konto";

                    // 4) Odświeżamy uprawnienia admina i filtry
                    UpdatePermissionUI();
                    ResetFilters();

                    // 5) Przywracamy rabat, jeśli wylosowany dziś
                    var spinDate = viewModel.CurrentUser.LastWheelSpinDate;
                    var discount = viewModel.CurrentUser.LastWheelDiscount;
                    if (spinDate.HasValue && spinDate.Value.Date == DateTime.Today && discount > 0)
                    {
                        viewModel.ApplyGlobalDiscount(discount);
                    }
                    else
                    {
                        // wygasły lub brak rabatu — wyzeruj w bazie
                        viewModel.CurrentUser.LastWheelDiscount = 0;
                        new UserViewModel().UpdateUserWheel(
                            viewModel.CurrentUser.Username,
                            spinDate ?? DateTime.Today,
                            0
                        );
                    }
                }
            }
            else
            {
                // 1) Pokaż okno konta zamiast od razu wylogowywać
                var account = new AccountWindow(viewModel.CurrentUser.Username) { Owner = this };
                account.ShowDialog();
                if (account.LoggedOut)
                {
                    // 2) Wylogowanie
                    viewModel.CurrentUser = null;
                    cartViewModel.CurrentUsername = "Gość";
                    LoginButton.Content = "Zaloguj się";
                    UpdatePermissionUI();
                    ResetFilters();
                }
            }
        }

        private void AdminPanel_Click(object sender, RoutedEventArgs e)
        {
            var panel = new AdminPanelWindow(viewModel);
            panel.ShowDialog();
        }
        private void WheelOfFortune_Click(object sender, RoutedEventArgs e)
        {
            // Sprawdź najpierw, czy user jest zalogowany
            if (viewModel.CurrentUser == null)
            {
                MessageBox.Show(
                    "Musisz być zalogowany, aby kręcić kołem fortuny.",
                    "Brak dostępu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var win = new WheelOfFortuneWindow(viewModel, new UserViewModel())
            {
                Owner = this
            };
            win.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)this.Resources["FadeInStoryboard"];
            storyboard.Begin(this);
        }

        private void UpdatePermissionUI()
        {
            bool isAdmin = viewModel.CurrentUser?.IsAdmin == true;

            AdminPanelButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }
        private void ResetFilters()
        {
            // ViewModel
            viewModel.SearchText = "";
            viewModel.SelectedCategoryId = 0;
            viewModel.SortBy = "Name";
            viewModel.SortDescending = false;
            viewModel.LoadProducts();

            // UI
            SearchBox.Text = "";
            CategoryFilterBox.SelectedIndex = 0;
        }

        private void ProductList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
