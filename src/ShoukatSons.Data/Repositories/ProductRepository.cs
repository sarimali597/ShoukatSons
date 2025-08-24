using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoukatSons.Core.Models;

namespace ShoukatSons.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product>
    {
        public ProductRepository(DatabaseContext db) : base(db) { }

        public Task<Product?> GetByBarcodeAsync(string barcode)
        {
            return _set.Include(p => p.Category)
                       .FirstOrDefaultAsync(p => p.Barcode == barcode);
        }
    }
}