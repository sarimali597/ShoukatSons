// ─────────────────────────────────────────────────────
// File: ViewModels/DashboardViewModel.cs
// ─────────────────────────────────────────────────────
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShoukatSons.Services.Interfaces;       // IReportService
using ShoukatSons.Services.Models;           // StockAlertDto
using ShoukatSons.UI.Services;               // IAlertPublisher
using ShoukatSons.UI.Services.Interfaces;    // INavigationService

namespace ShoukatSons.UI.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IReportService _reportService;
        private readonly INavigationService _nav;
        private readonly IAlertPublisher _alerts;

        public DashboardViewModel(
            IReportService reportService,
            INavigationService navigationService,
            IAlertPublisher alerts)
        {
            _reportService = reportService;
            _nav           = navigationService;
            _alerts        = alerts;

            Alerts                = new ObservableCollection<StockAlertDto>();
            LoadDashboardCommand  = new AsyncRelayCommand(LoadDashboardAsync);
            LoadAlertsCommand     = new AsyncRelayCommand(LoadAlertsAsync);
            DismissAlertCommand   = new AsyncRelayCommand<int>(DismissAlertAsync);

            ManageProductsCommand = new RelayCommand(() => _nav.ShowDialog<Views.ProductsView>());
            OpenPOSCommand        = new RelayCommand(() => _nav.ShowDialog<Views.POSWindow>());
            LabelGeneratorCommand = new RelayCommand(() => _nav.ShowDialog<Views.LabelGeneratorWindow>());
            ReportsCommand        = new RelayCommand(() => _nav.ShowDialog<Views.ReportsView>());
            SettingsCommand       = new RelayCommand(() => _nav.ShowDialog<Views.SettingsWindow>());
            ExitCommand           = new RelayCommand(() => _nav.CloseMainWindow());

            _alerts.AlertReceived += (_, alert) =>
                App.Current?.Dispatcher.Invoke(() => Alerts.Add(alert));

            _ = LoadDashboardAsync();
            _ = LoadAlertsAsync();
        }

        [ObservableProperty]
        private int productsCount;

        [ObservableProperty]
        private decimal todayTotal;

        [ObservableProperty]
        private int threshold = 5;

        // Must exactly match generated stub
        partial void OnThresholdChanged(int value)
            => _ = LoadAlertsAsync();

        public ObservableCollection<StockAlertDto> Alerts { get; }

        public IAsyncRelayCommand      LoadDashboardCommand  { get; }
        public IAsyncRelayCommand      LoadAlertsCommand     { get; }
        public IAsyncRelayCommand<int> DismissAlertCommand   { get; }

        public IRelayCommand ManageProductsCommand { get; }
        public IRelayCommand OpenPOSCommand        { get; }
        public IRelayCommand LabelGeneratorCommand { get; }
        public IRelayCommand ReportsCommand        { get; }
        public IRelayCommand SettingsCommand       { get; }
        public IRelayCommand ExitCommand           { get; }

        private async Task LoadDashboardAsync()
        {
            var ct = CancellationToken.None;
            ProductsCount = await _reportService.GetProductsCountAsync(ct);
            TodayTotal    = await _reportService.GetTodayTotalAsync(ct);
        }

        private async Task LoadAlertsAsync()
        {
            var ct   = CancellationToken.None;
            var list = await _reportService.GetLowStockAlertsAsync(Threshold, ct);

            Alerts.Clear();
            foreach (var a in list.OrderByDescending(a => a.AlertedAt))
                Alerts.Add(a);
        }

        private async Task DismissAlertAsync(int alertId)
        {
            await _reportService.DismissAlertAsync(alertId);
            var toRemove = Alerts.FirstOrDefault(a => a.Id == alertId);
            if (toRemove != null)
                Alerts.Remove(toRemove);
        }
    }
}