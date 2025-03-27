using System;
using SklepInternetowyWPF.Models;

public class CartItem
{
    public Product Product { get; set; }
    public int Quantity { get; set; }

    public decimal Total => Product.Price * Quantity;
}
