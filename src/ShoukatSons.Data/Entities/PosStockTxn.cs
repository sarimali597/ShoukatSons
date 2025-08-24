namespace ShoukatSons.Data.Entities
{
    public class StockTxn
    {
        public Guid Id { get; set; }                // Still a Guid for the transaction itself

        public int ProductId { get; set; }          // Changed from Guid → int to match Product.Id
        public Product Product { get; set; } = null!;

        public decimal QuantityChange { get; set; }

        // Match old name
        public StockTxnType TxnType { get; set; }   // Alias for Type
        public StockTxnType Type     { get; set; }  // Keep your current property

        public string? Reason { get; set; }

        // Add back older text fields if services still expect them
        public string? Reference { get; set; }
        public string? Note { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime CreatedAt    { get; set; }  // for legacy service code

        public Guid? CreatedByUserId { get; set; }

        public Guid? DocumentId { get; set; }
    }
}