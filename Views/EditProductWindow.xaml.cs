using Microsoft.Win32;
using SklepInternetowyWPF.Data;
using SklepInternetowyWPF.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

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
            if (!HasValidationErrors(this))
            {
                Product.StockMax = Product.Stock;

                using (var db = new AppDbContext())
                {
                    var existing = db.Products.FirstOrDefault(p => p.Id == Product.Id);
                    if (existing != null)
                    {
                        existing.Name = Product.Name;
                        existing.Description = Product.Description;
                        existing.Price = Product.Price;
                        existing.CategoryId = Product.CategoryId;
                        existing.Stock = Product.Stock;
                        existing.StockMax = Product.StockMax;
                        existing.ImagePath = Product.ImagePath;
                        db.SaveChanges();
                    }
                }

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Popraw wszystkie błędy przed zapisaniem.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private bool HasValidationErrors(DependencyObject obj)
        {
            return Validation.GetHasError(obj) ||
                   LogicalTreeHelper.GetChildren(obj).OfType<DependencyObject>().Any(HasValidationErrors);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)this.Resources["FadeInStoryboard"];
            storyboard.Begin(this);
        }

        private void StockBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }

        private void ImagePlaceholder_Click(object sender, MouseButtonEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Wybierz zdjęcie produktu",
                Filter = "Pliki graficzne (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    string projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                    string imagesFolder = Path.Combine(projectDir, "Images");
                    Directory.CreateDirectory(imagesFolder);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(dlg.FileName);
                    string destPath = Path.Combine(imagesFolder, fileName);

                    File.Copy(dlg.FileName, destPath, true);

                    Product.ImagePath = $"Images/{fileName}".Replace("\\", "/");

                    string runtimeImagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                    Directory.CreateDirectory(runtimeImagesFolder);
                    File.Copy(destPath, Path.Combine(runtimeImagesFolder, fileName), true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd podczas zapisywania obrazu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void PriceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            int pos = tb.SelectionStart;
            string normalized = tb.Text.Replace('.', ',');
            if (normalized != tb.Text)
            {
                tb.Text = normalized;
                tb.SelectionStart = pos;
            }
        }
        private void PriceBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = e.Text.FirstOrDefault();
            if (!char.IsDigit(c) && c != ',' && c != '.')
                e.Handled = true;
        }
        private void PriceBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!Regex.IsMatch(text, @"^[0-9\.,]+$"))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
