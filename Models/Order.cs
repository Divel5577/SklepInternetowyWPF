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

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Notes { get; set; }
        public string PaymentMethod { get; set; }

        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Total => Items.Sum(i => i.Total);
    }

}
