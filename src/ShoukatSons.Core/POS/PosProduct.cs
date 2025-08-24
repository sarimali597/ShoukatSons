namespace ShoukatSons.Core.POS
{
    public class PosProduct
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

        // ← New: When stock ≤ this value, trigger a low-stock alert
        public decimal LowStockThreshold { get; set; } = 0m;
    }
}