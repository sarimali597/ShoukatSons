// File: src/ShoukatSons.UI/Services/LowStockAlertService.cs
using Microsoft.Extensions.Hosting;
using ShoukatSons.Core.Models;
using ShoukatSons.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShoukatSons.UI.Services
{
    public class LowStockAlertService : BackgroundService
    {
        private readonly IStockAlertService _stockAlertService;

        public LowStockAlertService(IStockAlertService stockAlertService)
        {
            _stockAlertService = stockAlertService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var alerts = await _stockAlertService.CheckLowStockAsync();

                foreach (var alert in alerts)
                {
                    var coreAlert = new StockAlert
                    {
                        Id = alert.Id, // already Guid
                        ProductId = alert.ProductId,
                        Message = alert.Message,
                        Quantity = alert.Quantity,
                        AlertedAt = alert.AlertedAt
                    };

                    // TODO: push coreAlert to your notification hub / UI
                }

                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}