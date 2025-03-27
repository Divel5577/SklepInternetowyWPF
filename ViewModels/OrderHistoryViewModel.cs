using SklepInternetowyWPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;

namespace SklepInternetowyWPF.ViewModels
{
    public class OrderHistoryViewModel
    {
        private readonly string _connectionString = "Data Source=shop.db;Version=3;";
        public ObservableCollection<Order> Orders { get; set; } = new ObservableCollection<Order>();

        public OrderHistoryViewModel(string username)
        {
            LoadOrders(username);
        }

        public void LoadOrders(string username)
        {
            Orders.Clear();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string orderSql = "SELECT Id, Username, Date FROM Orders WHERE Username = @Username ORDER BY Date DESC";

                using (var orderCmd = new SQLiteCommand(orderSql, connection))
                {
                    orderCmd.Parameters.AddWithValue("@Username", username);
                    using (var reader = orderCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var orderId = reader.GetInt32(0);
                            var order = new Order
                            {
                                Id = orderId,
                                Username = reader.GetString(1),
                                Date = DateTime.Parse(reader.GetString(2)),
                                Items = new List<CartItem>()
                            };

                            LoadOrderItems(order, connection);
                            Orders.Add(order);
                        }
                    }
                }
            }
        }

        private void LoadOrderItems(Order order, SQLiteConnection connection)
        {
            string itemSql = @"
                SELECT oi.ProductId, oi.Quantity, p.Name, p.Price
                FROM OrderItems oi
                JOIN Products p ON p.Id = oi.ProductId
                WHERE oi.OrderId = @OrderId";

            using (var itemCmd = new SQLiteCommand(itemSql, connection))
            {
                itemCmd.Parameters.AddWithValue("@OrderId", order.Id);
                using (var reader = itemCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        order.Items.Add(new CartItem
                        {
                            Product = new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(2),
                                Price = (decimal)reader.GetDouble(3)
                            },
                            Quantity = reader.GetInt32(1)
                        });
                    }
                }
            }
        }
    }
}
