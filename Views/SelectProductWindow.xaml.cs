using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using SklepInternetowyWPF.Data;
using SklepInternetowyWPF.Models;

namespace SklepInternetowyWPF.Views
{
    public partial class SelectProductWindow : Window
    {
        public ObservableCollection<Product> Products { get; private set; }
        public Product SelectedProduct { get; private set; }

        public SelectProductWindow()
        {
            InitializeComponent();

            // 1) Pobierz wszystkie produkty z bazy
            using (var db = new AppDbContext())
            {
                Products = new ObservableCollection<Product>(db.Products.ToList());
            }

            // 2) Podłącz do UI
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Resources["FadeInStoryboard"] is Storyboard sb)
                sb.Begin(this);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListBox.SelectedItem is Product p)
            {
                SelectedProduct = p;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Wybierz produkt z listy.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
