using ShoukatSons.Data.Entities;
using System.Threading.Tasks;

namespace ShoukatSons.Services.Interfaces
{
    public interface IStockService
    {
        Task ReceiveStockAsync(int productId, int qty, decimal? unitCost, string? note, int userId);
        Task AdjustStockAsync(int productId, int qty, bool increase, string reason, int approverUserId);
    }
}