using SklepInternetowyWPF.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;

namespace SklepInternetowyWPF.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        private static readonly ObservableCollection<CartItem> cartItems = new ObservableCollection<CartItem>();

        public ObservableCollection<CartItem> CartItems { get; set; } = cartItems;
        public string CurrentUsername { get; set; }
        private readonly string _connectionString = "Data Source=shop.db;Version=3;";

        public decimal Total => CartItems.Sum(item => item.Total);

        public CartViewModel(string username)
        {
            CurrentUsername = string.IsNullOrWhiteSpace(username) ? "Gość" : username;
            CreateTables();
        }

        private void CreateTables()
        {
            var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string createOrders = @"CREATE TABLE IF NOT EXISTS Orders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL,
                Date TEXT NOT NULL
            );";

            string createOrderItems = @"CREATE TABLE IF NOT EXISTS OrderItems (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OrderId INTEGER,
                ProductId INTEGER,
                Quantity INTEGER,
                FOREIGN KEY (OrderId) REFERENCES Orders(Id),
                FOREIGN KEY (ProductId) REFERENCES Products(Id)
            );";

            new SQLiteCommand(createOrders, connection).ExecuteNonQuery();
            new SQLiteCommand(createOrderItems, connection).ExecuteNonQuery();
            connection.Close();
        }

        public void AddToCart(Product product)
        {
            var existing = CartItems.FirstOrDefault(c => c.Product.Id == product.Id);
            if (existing != null)
                existing.Quantity++;
            else
                CartItems.Add(new CartItem { Product = product, Quantity = 1 });

            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(Total));
        }

        public void ClearCart()
        {
            CartItems.Clear();
            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(Total));
        }

        public void PlaceOrder()
        {
            if (CartItems.Count == 0) return;

            var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            var insertOrder = new SQLiteCommand("INSERT INTO Orders (Username, Date) VALUES (@Username, @Date);", connection);
            insertOrder.Parameters.AddWithValue("@Username", CurrentUsername);
            insertOrder.Parameters.AddWithValue("@Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            insertOrder.ExecuteNonQuery();

            long orderId = connection.LastInsertRowId;

            foreach (var item in CartItems)
            {
                var insertItem = new SQLiteCommand("INSERT INTO OrderItems (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity);", connection);
                insertItem.Parameters.AddWithValue("@OrderId", orderId);
                insertItem.Parameters.AddWithValue("@ProductId", item.Product.Id);
                insertItem.Parameters.AddWithValue("@Quantity", item.Quantity);
                insertItem.ExecuteNonQuery();
            }

            connection.Close();
            ClearCart();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
