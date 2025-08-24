using System.Collections.Generic;

namespace ShoukatSons.Core.Entities
{
    /// <summary>
    /// Represents a product category in the Shoukat Sons retail system.
    /// </summary>
    public class CategoryEntity
    {
        public int Id { get; set; }

        public required string Name { get; set; } = string.Empty;

        // Navigation back to products
        public ICollection<ProductEntity> Products { get; set; }
            = new List<ProductEntity>();
    }
}