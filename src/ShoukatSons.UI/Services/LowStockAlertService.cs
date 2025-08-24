using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using ShoukatSons.Services;
using ShoukatSons.Services.Models;

namespace ShoukatSons.UI.Services
{
    public class LowStockAlertService : BackgroundService
    {
        private readonly IStockAlertService _stockAlerts;
        private readonly IAlertPublisher _publisher;

        public LowStockAlertService(IStockAlertService stockAlertService, IAlertPublisher publisher)
        {
            _stockAlerts = stockAlertService;
            _publisher = publisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Initial delay to let UI start
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var alerts = await _stockAlerts.CheckLowStockAsync();
                    foreach (var a in alerts)
                    {
                        var dto = new StockAlertDto
                        {
                            Id = a.Id,
                            ProductId = a.ProductId,
                            Message = a.Message,
                            Quantity = a.Quantity,
                            AlertedAt = a.AlertedAt
                        };
                        _publisher.Publish(dto);
                    }
                }
                catch
                {
                    // swallow and continue
                }

                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}