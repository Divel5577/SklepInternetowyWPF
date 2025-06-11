using System;
using System.Data.SQLite;
using SklepInternetowyWPF.Models;

namespace SklepInternetowyWPF.ViewModels
{
    public class UserViewModel
    {
        private readonly string _connectionString = "Data Source=shop.db;Version=3;";

        public UserViewModel()
        {
            CreateTables();
            AddUser("admin", "admin123", true);
            AddUser("user", "user123", false);
        }

        private void CreateTables()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string createUsers = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Password TEXT NOT NULL,
                        IsAdmin INTEGER NOT NULL,
                        LastWheelSpinDate TEXT,
                        LastWheelDiscount    INTEGER
                    );";

                using (var cmd = new SQLiteCommand(createUsers, connection))
                    cmd.ExecuteNonQuery();
            }
        }

        public void AddUser(string username, string password, bool isAdmin)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string sql = "INSERT OR IGNORE INTO Users (Username, Password, IsAdmin) VALUES (@Username, @Password, @IsAdmin)";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password); // UWAGA: niehaszowane!
                    cmd.Parameters.AddWithValue("@IsAdmin", isAdmin ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public User GetUser(string username, string password)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT Id, Username, Password, IsAdmin, LastWheelSpinDate, LastWheelDiscount FROM Users WHERE Username = @Username AND Password = @Password";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                IsAdmin = reader.GetInt32(3) == 1,
                                LastWheelSpinDate = reader.IsDBNull(4)
                                                       ? (DateTime?)null
                                                       : DateTime.Parse(reader.GetString(4)),
                                LastWheelDiscount = reader.IsDBNull(5)
                                                ? 0
                                                : reader.GetInt32(5)
                            };
                        }
                    }
                }
            }
            return null;
        }
        public void UpdateLastSpinDate(string username, DateTime date)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = @"
          UPDATE Users
             SET LastWheelSpinDate = @Date
           WHERE Username = @Username";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateUserWheel(string username, DateTime spinDate, int discount)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = @"
            UPDATE Users
               SET LastWheelSpinDate = @D,
                   LastWheelDiscount = @Disc
             WHERE Username = @U";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@D", spinDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Disc", discount);
                    cmd.Parameters.AddWithValue("@U", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
