﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

public class Product : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    [NotMapped]
    public string CategoryName { get; set; }
    public int Stock { get; set; }
    public int StockMax { get; set; }
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
            if (StockMax == 0) return "";
            double percent = (double)Stock / StockMax;
            if (percent < 0.25)
                return "Mało";
            if (percent <= 0.6)
                return "Średnio";
            return "Dużo";
        }
    }
}
