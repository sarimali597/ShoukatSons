using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoukatSons.Core.Models;

namespace ShoukatSons.Data.Repositories
{
    public class SaleRepository : GenericRepository<Sale>
    {
        public SaleRepository(DatabaseContext db) : base(db) { }

        public Task<Sale?> GetWithItemsAsync(int id)
        {
            return _set.Include(s => s.Items)
                       .ThenInclude(i => i.Product)
                       .FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task<decimal> GetTodaySalesTotalAsync()
        {
            var today = System.DateTime.UtcNow.Date;
            return _set.Where(s => s.SaleDate >= today && s.SaleDate < today.AddDays(1))
                       .SumAsync(s => s.Total);
        }
    }
}