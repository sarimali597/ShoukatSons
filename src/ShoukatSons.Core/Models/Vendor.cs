using System.Collections.Generic;

namespace ShoukatSons.Core.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public ICollection<Product> SuppliedProducts { get; set; } = new List<Product>();
    }
}