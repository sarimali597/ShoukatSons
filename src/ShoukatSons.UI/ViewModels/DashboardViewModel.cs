using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;  // for ObservableObject & [ObservableProperty]
using ShoukatSons.Services.Interfaces;
using ShoukatSons.Services.Models;



namespace ShoukatSons.UI.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly IReportService _reportService;

        public DashboardViewModel(IReportService reportService)
        {
            _reportService       = reportService;
            DismissAlertCommand  = new RelayCommand<int>(async id => await DismissAlertAsync(id));
            _ = LoadAsync();
        }

        //– Properties for dashboard metrics

        private int _productsCount;
        public int ProductsCount
        {
            get => _productsCount;
            set
            {
                if (_productsCount == value) return;
                _productsCount = value;
                OnPropertyChanged();
            }
        }

        private decimal _todayTotal;
        public decimal TodayTotal
        {
            get => _todayTotal;
            set
            {
                if (_todayTotal == value) return;
                _todayTotal = value;
                OnPropertyChanged();
            }
        }

        //– Threshold for low-stock alerts (can be bound in the UI)
        private int _threshold = 5;
        public int Threshold
        {
            get => _threshold;
            set
            {
                if (_threshold == value) return;
                _threshold = value;
                OnPropertyChanged();
                _ = LoadAsync();    // reload alerts when threshold changes
            }
        }

        //– Collection of alerts displayed on the dashboard
        public ObservableCollection<StockAlertDto> Alerts { get; } = new();

        //– Command to dismiss an alert by its Id
        public RelayCommand<int> DismissAlertCommand { get; }

        //– Load all dashboard data in one go
        private async Task LoadAsync()
        {
            try
            {
                ProductsCount = await _reportService.GetProductsCountAsync();
                TodayTotal    = await _reportService.GetTodayTotalAsync();

                var alerts = await _reportService.GetLowStockAlertsAsync(Threshold);
                Alerts.Clear();
                foreach (var alert in alerts)
                {
                    Alerts.Add(alert);
                }
            }
            catch (Exception)
            {
                // consider logging or showing a UI notification
                throw;
            }
        }

        //– Dismiss a single alert and remove it from the UI
        private async Task DismissAlertAsync(int alertId)
        {
            try
            {
                await _reportService.DismissAlertAsync(alertId);

                var toRemove = Alerts.FirstOrDefault(a => a.Id == alertId);
                if (toRemove != null)
                    Alerts.Remove(toRemove);
            }
            catch (Exception)
            {
                // consider logging or showing a UI notification
                throw;
            }
        }

        //–– INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}