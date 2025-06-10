using SklepInternetowyWPF.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Windows;

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
                Date TEXT NOT NULL,
                FirstName TEXT,
                LastName TEXT,
                Phone TEXT,
                Street TEXT,
                PostalCode TEXT,
                City TEXT,
                Notes TEXT,
                PaymentMethod TEXT
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
        }

        public bool AddToCart(Product product)
        {
            int totalInCart = CartItems.Where(x => x.Product.Id == product.Id).Sum(x => x.Quantity);

            if (totalInCart >= product.Stock)
                return false;

            var existing = CartItems.FirstOrDefault(c => c.Product.Id == product.Id);
            if (existing != null)
                existing.Quantity++;
            else
                CartItems.Add(new CartItem { Product = product, Quantity = 1 });

            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(Total));
            return true;
        }

        public void ClearCart()
        {
            CartItems.Clear();
            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(Total));
        }

        public void PlaceOrder(string firstName, string lastName, string phone, string street, string postalCode, string city, string notes, string paymentMethod)
        {
            if (CartItems.Count == 0) return;

            var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            var insertOrder = new SQLiteCommand(@"
                INSERT INTO Orders (Username, Date, FirstName, LastName, Phone, Street, PostalCode, City, Notes, PaymentMethod)
                VALUES (@Username, @Date, @FirstName, @LastName, @Phone, @Street, @PostalCode, @City, @Notes, @PaymentMethod);", connection);

            insertOrder.Parameters.AddWithValue("@Username", CurrentUsername);
            insertOrder.Parameters.AddWithValue("@Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            insertOrder.Parameters.AddWithValue("@FirstName", firstName);
            insertOrder.Parameters.AddWithValue("@LastName", lastName);
            insertOrder.Parameters.AddWithValue("@Phone", phone);
            insertOrder.Parameters.AddWithValue("@Street", street);
            insertOrder.Parameters.AddWithValue("@PostalCode", postalCode);
            insertOrder.Parameters.AddWithValue("@City", city);
            insertOrder.Parameters.AddWithValue("@Notes", notes);
            insertOrder.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
            insertOrder.ExecuteNonQuery();

            long orderId = connection.LastInsertRowId;

            foreach (var item in CartItems)
            {
                var insertItem = new SQLiteCommand("INSERT INTO OrderItems (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, @Quantity);", connection);
                insertItem.Parameters.AddWithValue("@OrderId", orderId);
                insertItem.Parameters.AddWithValue("@ProductId", item.Product.Id);
                insertItem.Parameters.AddWithValue("@Quantity", item.Quantity);
                insertItem.ExecuteNonQuery();

                var updateStock = new SQLiteCommand("UPDATE Products SET Stock = Stock - @Quantity WHERE Id = @ProductId;", connection);
                updateStock.Parameters.AddWithValue("@Quantity", item.Quantity);
                updateStock.Parameters.AddWithValue("@ProductId", item.Product.Id);
                updateStock.ExecuteNonQuery();
            }

            ClearCart();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
