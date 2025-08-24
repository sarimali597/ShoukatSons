using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShoukatSons.Services.Models;

namespace ShoukatSons.Services.Interfaces
{
    public interface IReportService
    {
        Task<int> GetProductsCountAsync(CancellationToken cancellationToken = default);
        Task<decimal> GetTodayTotalAsync(CancellationToken cancellationToken = default);

        Task<List<StockAlertDto>> GetLowStockAlertsAsync(
            int threshold,
            CancellationToken cancellationToken = default);

        Task DismissAlertAsync(
            int alertId,                            // ← int here
            CancellationToken cancellationToken = default);
    }
}