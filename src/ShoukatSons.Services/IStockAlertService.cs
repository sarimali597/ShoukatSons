using System.Collections.Generic;
using System.Threading.Tasks;
using ShoukatSons.Core.Models;

namespace ShoukatSons.Services
{
    public interface IStockAlertService
    {
        Task<List<StockAlert>> GetActiveAlertsAsync();
        Task DismissAsync(int alertId);              // ← int, not Guid
        Task<List<StockAlert>> CheckLowStockAsync();
    }
}