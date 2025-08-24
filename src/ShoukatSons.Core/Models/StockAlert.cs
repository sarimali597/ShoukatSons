using System;

namespace ShoukatSons.Core.Models
{
    public class StockAlert
    {
        // Primary key
        public int Id { get; set; }

        // FK → Product
        public int ProductId { get; set; }

        // Navigation (fixes EF mapping in DataContext)
        public Product Product { get; set; } = null!;

        // how many are left
        public int Quantity { get; set; }

        // descriptive message
        public string Message { get; set; } = string.Empty;

        // when this alert fired
        public DateTime AlertedAt { get; set; }

        // optional dismissed timestamp
        public DateTime? DismissedAt { get; set; }
    }
}