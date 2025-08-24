using System;
using ShoukatSons.Core.Models;

namespace ShoukatSons.Core.POS
{
    public class PosDocumentLine
    {
        public Guid Id { get; set; } // Unique identifier for the document line
        public int ProductId { get; set; } // Changed from Guid → int to match Product.Id
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineDiscount { get; set; }

        public Product? Product { get; set; } // Navigation property to Product

        public decimal LineTotal => (Quantity * UnitPrice) - LineDiscount;
    }
}