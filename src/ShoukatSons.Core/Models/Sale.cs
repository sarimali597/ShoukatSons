// File: src/ShoukatSons.Core/Models/Sale.cs

using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoukatSons.Core.Models
{
    public class Sale
    {
        public int    Id        { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public int    UserId    { get; set; }
        public User?  User      { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax      { get; set; }
        public decimal Total    { get; set; }
        public string  Notes    { get; set; } = string.Empty;

        public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

        // ← new alias for existing service calls
        public decimal TotalAmount => Total;
    }
}