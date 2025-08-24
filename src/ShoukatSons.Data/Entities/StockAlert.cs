using System;
using System.ComponentModel.DataAnnotations;

namespace ShoukatSons.Data.Entities
{
    public class StockAlert
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        [MaxLength(400)]
        public string Message { get; set; } = string.Empty;

        // renamed from CreatedAtUtc
        public DateTime AlertedAt { get; set; }

        // renamed from DismissedAtUtc
        public DateTime? DismissedAt { get; set; }
    }
}