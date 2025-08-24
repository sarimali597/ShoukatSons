using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoukatSons.Data;
using ShoukatSons.Services.Interfaces;
using ShoukatSons.Services.Models;

namespace ShoukatSons.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<ReportService> _logger;

        public ReportService(DatabaseContext context, ILogger<ReportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> GetProductsCountAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Products.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting products");
                throw;
            }
        }

        public async Task<decimal> GetTodayTotalAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.Today;
            try
            {
                return await _context.Sales
                    .Where(s => s.SaleDate >= today && s.SaleDate < today.AddDays(1))
                    .SumAsync(s => (decimal?)s.TotalAmount, cancellationToken)
                    ?? 0m;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing today's total");
                throw;
            }
        }

        public async Task<List<StockAlertDto>> GetLowStockAlertsAsync(
            int threshold,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.StockAlerts
                    .Where(sa => sa.Quantity <= threshold)
                    .Select(sa => new StockAlertDto
                    {
                        Id        = sa.Id,
                        ProductId = sa.ProductId,
                        Message   = sa.Message,
                        Quantity  = sa.Quantity,
                        AlertedAt = sa.AlertedAt
                    })
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching low-stock alerts (threshold={Threshold})",
                    threshold);
                throw;
            }
        }

        public async Task DismissAlertAsync(
            int alertId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var alert = await _context.StockAlerts
                    .FirstOrDefaultAsync(a => a.Id == alertId, cancellationToken);

                if (alert == null)
                {
                    _logger.LogWarning(
                        "No stock alert found with Id={AlertId}",
                        alertId);
                    return;
                }

                _context.StockAlerts.Remove(alert);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Dismissed stock alert Id={AlertId}",
                    alertId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error dismissing alert Id={AlertId}",
                    alertId);
                throw;
            }
        }
    }
}