using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoukatSons.Core.Models;
using ShoukatSons.Data;

namespace ShoukatSons.Services
{
    public class StockAlertService : IStockAlertService
    {
        private readonly DatabaseContext _context;

        public StockAlertService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<StockAlert>> GetActiveAlertsAsync()
        {
            return await _context.StockAlerts
                .Select(sa => new StockAlert
                {
                    Id        = sa.Id,
                    ProductId = sa.ProductId,
                    Quantity  = sa.Quantity,
                    Message   = sa.Message,
                    AlertedAt = sa.AlertedAt
                })
                .ToListAsync();
        }

        public async Task DismissAsync(int alertId)      // ← int here
        {
            var alert = await _context.StockAlerts
                .FirstOrDefaultAsync(a => a.Id == alertId);

            if (alert != null)
            {
                _context.StockAlerts.Remove(alert);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<StockAlert>> CheckLowStockAsync()
        {
            return await _context.Products
                .Where(p => p.StockQuantity <= p.LowStockThreshold)
                .Select(p => new StockAlert
                {
                    Id        = p.Id,
                    ProductId = p.Id,
                    Quantity  = (int)p.StockQuantity,
                    Message   = $"Low stock for '{p.Name}': only {p.StockQuantity} left",
                    AlertedAt = DateTime.UtcNow
                })
                .ToListAsync();
        }
    }
}