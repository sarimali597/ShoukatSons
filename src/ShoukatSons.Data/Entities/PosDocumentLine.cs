using System;

namespace ShoukatSons.Data.Entities
{
    public class DocumentLine
    {
        public Guid Id { get; set; } // Still a Guid for the line itself

        public Guid DocumentId { get; set; }
        public Document Document { get; set; } = null!;

        public int ProductId { get; set; } // Changed from Guid → int to match Product.Id
        public Product Product { get; set; } = null!;

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineDiscount { get; set; }
        public decimal LineTotal { get; set; }
    }
}