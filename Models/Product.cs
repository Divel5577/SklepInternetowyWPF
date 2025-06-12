using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

public class Product : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public int Id { get; set; }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    private string _description;
    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    private decimal _price;
    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                _price = value;
                OnPropertyChanged(nameof(Price));
            }
        }
    }

    private int _categoryId;
    public int CategoryId
    {
        get => _categoryId;
        set
        {
            if (_categoryId != value)
            {
                _categoryId = value;
                OnPropertyChanged(nameof(CategoryId));
            }
        }
    }

    [NotMapped]
    private string _categoryName;
    public string CategoryName
    {
        get => _categoryName;
        set
        {
            if (_categoryName != value)
            {
                _categoryName = value;
                OnPropertyChanged(nameof(CategoryName));
            }
        }
    }

    private int _stock;
    public int Stock
    {
        get => _stock;
        set
        {
            if (_stock != value)
            {
                _stock = value;
                OnPropertyChanged(nameof(Stock));
                OnPropertyChanged(nameof(StockDescription));
            }
        }
    }

    private int _stockMax;
    public int StockMax
    {
        get => _stockMax;
        set
        {
            if (_stockMax != value)
            {
                _stockMax = value;
                OnPropertyChanged(nameof(StockMax));
                OnPropertyChanged(nameof(StockDescription));
            }
        }
    }

    private string _imagePath;
    public string ImagePath
    {
        get => _imagePath;
        set
        {
            if (_imagePath != value)
            {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }
    }

    public string StockDescription
    {
        get
        {
            if (Stock == 0)
                return "Brak";
            if (StockMax == 0)
                return "";
            double pct = (double)Stock / StockMax;
            if (pct < 0.25)
                return "Mało";
            if (pct <= 0.6)
                return "Średnio";
            return "Dużo";
        }
    }
}
