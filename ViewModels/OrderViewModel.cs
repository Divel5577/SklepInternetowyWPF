using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using SklepInternetowyWPF.Models;

namespace SklepInternetowyWPF.ViewModels
{
    public class OrderViewModel
    {
        public ObservableCollection<Order> Orders { get; set; } = new ObservableCollection<Order>();
        private readonly string _connectionString = "Data Source=shop.db;Version=3;";

        public void LoadOrders(string username)
        {
            Orders.Clear();

            var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            string query = @"
                SELECT o.Id, o.Date, p.Id, p.Name, p.Description, p.Price, p.CategoryId, oi.Quantity
                FROM Orders o
                JOIN OrderItems oi ON o.Id = oi.OrderId
                JOIN Products p ON p.Id = oi.ProductId
                WHERE o.Username = @Username
                ORDER BY o.Date DESC";

            var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Username", username);
            var reader = cmd.ExecuteReader();

            Dictionary<int, Order> orderMap = new Dictionary<int, Order>();

            while (reader.Read())
            {
                int orderId = reader.GetInt32(0);
                DateTime date = DateTime.Parse(reader.GetString(1));

                if (!orderMap.ContainsKey(orderId))
                {
                    orderMap[orderId] = new Order
                    {
                        Id = orderId,
                        Username = username,
                        Date = date,
                        Items = new List<CartItem>()
                    };
                }

                var product = new Product
                {
                    Id = reader.GetInt32(2),
                    Name = reader.GetString(3),
                    Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Price = (decimal)reader.GetDouble(5),
                    CategoryId = reader.GetInt32(6)
                };

                var quantity = reader.GetInt32(7);

                orderMap[orderId].Items.Add(new CartItem
                {
                    Product = product,
                    Quantity = quantity
                });
            }

            connection.Close();

            foreach (var order in orderMap.Values)
            {
                Orders.Add(order);
            }
        }
    }
}
