using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using SklepInternetowyWPF.Models;

namespace SklepInternetowyWPF.ViewModels
{
    public class ProductViewModel : INotifyPropertyChanged
    {
        private readonly string _connectionString = "Data Source=shop.db;Version=3;";

        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
        public User CurrentUser { get; set; }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        private int _selectedCategoryId = 0;
        public int SelectedCategoryId
        {
            get => _selectedCategoryId;
            set
            {
                _selectedCategoryId = value;
                OnPropertyChanged(nameof(SelectedCategoryId));
            }
        }

        private string _sortBy = "Name";
        public string SortBy
        {
            get => _sortBy;
            set
            {
                _sortBy = value;
                OnPropertyChanged(nameof(SortBy));
                OnPropertyChanged(nameof(NameHeader));
                OnPropertyChanged(nameof(PriceHeader));
            }
        }

        private bool _sortDescending = false;
        public bool SortDescending
        {
            get => _sortDescending;
            set
            {
                _sortDescending = value;
                OnPropertyChanged(nameof(SortDescending));
                OnPropertyChanged(nameof(NameHeader));
                OnPropertyChanged(nameof(PriceHeader));
            }
        }

        public string NameHeader => SortBy == "Name" ? $"Nazwa {(SortDescending ? "▼" : "▲")}" : "Nazwa";
        public string PriceHeader => SortBy == "Price" ? $"Cena {(SortDescending ? "▼" : "▲")}" : "Cena";

        public ProductViewModel()
        {
            CreateTables();
            LoadCategories();
            LoadProducts();
        }

        private void CreateTables()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string createCategories = @"
                    CREATE TABLE IF NOT EXISTS Categories (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL UNIQUE
                    );";

                string createProducts = @"
                    CREATE TABLE IF NOT EXISTS Products (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Description TEXT,
                        Price REAL NOT NULL,
                        Stock INTEGER NOT NULL,
                        StockMax INTEGER NOT NULL,
                        CategoryId INTEGER,
                        FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
                    );";


                using (var cmd = new SQLiteCommand(createCategories, connection))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SQLiteCommand(createProducts, connection))
                    cmd.ExecuteNonQuery();
            }
        }

        public void LoadCategories()
        {
            Categories.Clear();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Categories";
                using (var cmd = new SQLiteCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Categories.Add(new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }

            if (Categories.Count == 0)
            {
                AddCategory("Spożywcze");
                AddCategory("Elektronika");
                AddCategory("Ubrania");
                LoadCategories();
            }
        }

        public void LoadProducts()
        {
            Products.Clear();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string orderColumn = SortBy == "Price" ? "p.Price" : "p.Name";
                string orderDirection = SortDescending ? "DESC" : "ASC";

                string sql = $@"
                    SELECT p.Id, p.Name, p.Description, p.Price, p.Stock, p.StockMax, c.Id, c.Name
                    FROM Products p
                    LEFT JOIN Categories c ON p.CategoryId = c.Id
                    WHERE (@Search = '' OR p.Name LIKE @Search)
                      AND (@CategoryId = 0 OR p.CategoryId = @CategoryId)
                    ORDER BY {orderColumn} {orderDirection}";

                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    string search = SearchText ?? "";
                    int categoryId = SelectedCategoryId;

                    cmd.Parameters.AddWithValue("@Search", $"%{search}%");
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Products.Add(new Product
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Price = (decimal)reader.GetDouble(3),
                                Stock = reader.GetInt32(4),
                                StockMax = reader.GetInt32(5),
                                CategoryId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                CategoryName = reader.IsDBNull(7) ? "" : reader.GetString(7)
                            });
                        }
                    }
                }
            }

            OnPropertyChanged(nameof(NameHeader));
            OnPropertyChanged(nameof(PriceHeader));
        }

        public void SaveProduct(Product product)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = product.Id == 0
                    ? "INSERT INTO Products (Name, Description, Price, CategoryId, Stock, StockMax) VALUES (@Name, @Description, @Price, @CategoryId, @Stock, @StockMax)"
                    : "UPDATE Products SET Name = @Name, Description = @Description, Price = @Price, CategoryId = @CategoryId, Stock = @Stock, StockMax = @StockMax WHERE Id = @Id";

                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    if (product.Id != 0)
                        cmd.Parameters.AddWithValue("@Id", product.Id);

                    cmd.Parameters.AddWithValue("@Name", product.Name);
                    cmd.Parameters.AddWithValue("@Description", product.Description);
                    cmd.Parameters.AddWithValue("@Price", product.Price);
                    cmd.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                    cmd.Parameters.AddWithValue("@Stock", product.Stock);
                    cmd.Parameters.AddWithValue("@StockMax", product.StockMax);

                    cmd.ExecuteNonQuery();

                    if (product.Id == 0)
                        product.Id = (int)connection.LastInsertRowId;
                    if (product.StockMax == 0 || product.Stock > product.StockMax)
                        product.StockMax = product.Stock;

                }
            }

            LoadProducts();
        }

        public void DeleteProduct(Product product)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Products WHERE Id = @Id";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", product.Id);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadProducts();
        }

        public void AddCategory(string name)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "INSERT OR IGNORE INTO Categories (Name) VALUES (@Name)";
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
