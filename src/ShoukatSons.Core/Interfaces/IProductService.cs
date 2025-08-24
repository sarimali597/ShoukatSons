using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShoukatSons.Core.Models;

namespace ShoukatSons.Core.Interfaces
{
    /// <summary>
    /// Contract for CRUD operations on products.
    /// </summary>
    public interface IProductService
    {
        Task<IEnumerable<Product>> ListAsync(
            CancellationToken cancellationToken = default);

        Task<Product?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Product product,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Product product,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}