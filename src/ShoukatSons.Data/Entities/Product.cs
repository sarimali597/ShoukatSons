namespace ShoukatSons.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }    // Legacy nav property

        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal LowStockThreshold { get; set; } = 0m;

        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Legacy/compatibility props
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public bool IsDiscontinued { get; set; }

        public ICollection<StockTxn> StockTxns { get; set; } = new List<StockTxn>();
    }
}