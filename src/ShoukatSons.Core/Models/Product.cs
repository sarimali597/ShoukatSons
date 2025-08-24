using System;

namespace ShoukatSons.Core.Models
{
    public class Product
    {
        public int      Id                 { get; set; }
        public string   Barcode            { get; set; } = string.Empty;
        public string   Name               { get; set; } = string.Empty;
        public int      CategoryId         { get; set; }
        public Category? Category          { get; set; }

        // Optional SKU (kept for future use, not required by seeder)
        public string   Sku                { get; set; } = string.Empty;

        public decimal  PurchasePrice      { get; set; }
        public decimal  SalePrice          { get; set; }

        public int      StockQuantity      { get; set; }
        
        // New low-stock threshold per product
        public int      LowStockThreshold  { get; set; }

        public string   Size               { get; set; } = string.Empty;
        public string   Color              { get; set; } = string.Empty;

        public DateTime CreatedAt          { get; set; } = DateTime.UtcNow;
        public bool     IsActive           { get; set; } = true;
    }
}