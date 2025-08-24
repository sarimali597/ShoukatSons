using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShoukatSons.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShoukatSons.Services.Services
{
    /// <summary>
    /// Background service checks for low stock that periodically
    /// and logs or triggers alerts.
    /// </summary>
    public class LowStockAlertService : BackgroundService
    {
        private readonly IStockAlertService _stockAlertService;
        private readonly ILogger<LowStockAlertService> _logger;

        public LowStockAlertService(IStockAlertService stockAlertService, ILogger<LowStockAlertService> logger)
        {
            _stockAlertService = stockAlertService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LowStockAlertService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var alerts = await _stockAlertService.CheckLowStockAsync();

                    if (alerts.Count > 0)
                    {
                        foreach (var alert in alerts)
                        {
                            _logger.LogWarning($"Low stock: {alert.Message}");
                            // Optional: Save to DB, send notification, or update UI
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking low stock.");
                }

                // Wait 5 minutes before checking again
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("LowStockAlertService stopped.");
        }
    }
}