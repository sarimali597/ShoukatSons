using System;
using ShoukatSons.Services.Models;

namespace ShoukatSons.UI.Services
{
    public interface IAlertPublisher
    {
        event EventHandler<StockAlertDto>? AlertReceived;
        void Publish(StockAlertDto alert);
    }

    public class AlertPublisher : IAlertPublisher
    {
        public event EventHandler<StockAlertDto>? AlertReceived;
        public void Publish(StockAlertDto alert) => AlertReceived?.Invoke(this, alert);
    }
}