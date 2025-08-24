using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoukatSons.Core.Interfaces;

namespace ShoukatSons.Data.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly DatabaseContext _db;
        protected readonly DbSet<T> _set;

        public GenericRepository(DatabaseContext db)
        {
            _db = db;
            _set = _db.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await _set.FindAsync(id);

        public async Task<IReadOnlyList<T>> GetAllAsync() => await _set.AsNoTracking().ToListAsync();

        public async Task<T> AddAsync(T entity)
        {
            await _set.AddAsync(entity);
            return entity;
        }

        public Task UpdateAsync(T entity)
        {
            _set.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _set.Remove(entity);
            return Task.CompletedTask;
        }

        public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();
    }
}