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
                        IsAdmin INTEGER NOT NULL
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
                string sql = "SELECT Id, Username, Password, IsAdmin FROM Users WHERE Username = @Username AND Password = @Password";
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
                                IsAdmin = reader.GetInt32(3) == 1
                            };
                        }
                    }
                }
            }
            return null;
        }

    }
}
