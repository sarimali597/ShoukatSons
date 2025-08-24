// File: src/ShoukatSons.Core/Entities/ProductEntity.cs
namespace ShoukatSons.Core.Entities
{
    public class ProductEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public CategoryEntity Category { get; set; } = default!;

        // ✅ Add these new properties
        public string? Sku { get; set; }
        public string? Barcode { get; set; }
        public decimal Price { get; set; }
        public decimal? CostPrice { get; set; }
        public int StockQuantity { get; set; }
    }
}