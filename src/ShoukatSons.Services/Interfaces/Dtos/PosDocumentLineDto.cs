using System;

namespace ShoukatSons.Services.Interfaces.Dtos
{
    public class PosDocumentLineDto
    {
        public Guid Id { get; set; } // This can remain Guid if it's just the document line's own identifier
        public int ProductId { get; set; } // Changed to match Product from Guid → int.Id
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineDiscount { get; set; }

        // Optional: replace object with ProductDto if you want strong typing
        public object? Product { get; set; }

        public decimal LineTotal { get; set; }
    }
}