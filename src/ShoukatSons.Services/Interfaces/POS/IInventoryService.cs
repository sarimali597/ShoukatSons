using System;
using System.Threading;
using System.Threading.Tasks;
using ShoukatSons.Core.POS;

namespace ShoukatSons.Services.Interfaces.POS
{
    public interface IInventoryService
    {
        Task<int> GetOnHandAsync(string sku, CancellationToken ct);
        Task AdjustAsync(
            Guid productId,
            int quantityChange,
            PosStockMovementReason reason,
            Guid? relatedDocumentId,
            CancellationToken ct
        );
    }
}