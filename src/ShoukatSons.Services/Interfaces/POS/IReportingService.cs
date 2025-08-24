using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShoukatSons.Core.POS;

namespace ShoukatSons.Services.Interfaces.POS
{
    public interface IReportingService
    {
        Task<decimal> GetTotalSalesAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct);
        Task<List<PosDocumentLine>> GetTopSellingProductsAsync(
            DateTime fromUtc,
            DateTime toUtc,
            int topN,
            CancellationToken ct
        );
    }
}