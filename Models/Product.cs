public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int Stock { get; set; }
    public int StockMax { get; set; }

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
