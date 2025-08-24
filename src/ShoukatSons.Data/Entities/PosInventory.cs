using System;

namespace ShoukatSons.Data.Entities
{
    public class Inventory
    {
        public Guid Id { get; set; } // Still a Guid for the inventory record itself

        public int ProductId { get; set; } // Changed from Guid → int to match Product.Id
        public Product Product { get; set; } = null!;

        public decimal QuantityOnHand { get; set; }
        public decimal ReorderLevel { get; set; }

        public DateTime LastUpdatedUtc { get; set; }
    }
}