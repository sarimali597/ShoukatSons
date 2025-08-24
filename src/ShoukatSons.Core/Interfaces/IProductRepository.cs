// File: src/ShoukatSons.Core/Interfaces/IProductRepository.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShoukatSons.Core.Entities;

namespace ShoukatSons.Core.Interfaces
{
    /// <summary>
    /// Defines persistence operations for <see cref="ProductEntity"/>.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Retrieves all products.
        /// </summary>
        Task<IEnumerable<ProductEntity>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a product by ID, or null if not found.
        /// </summary>
        Task<ProductEntity?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new product and returns the persisted entity.
        /// </summary>
        Task<ProductEntity> AddAsync(
            ProductEntity entity,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing product and returns the updated entity.
        /// </summary>
        Task<ProductEntity> UpdateAsync(
            ProductEntity entity,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}