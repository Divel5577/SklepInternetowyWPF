using System;
using System.Collections.Generic;
using System.Linq;

namespace SklepInternetowyWPF.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Total => Items.Sum(i => i.Total);
    }
}
