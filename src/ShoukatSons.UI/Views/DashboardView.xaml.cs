using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ShoukatSons.Services.Interfaces;
using ShoukatSons.UI.ViewModels;

namespace ShoukatSons.UI.Views
{
    public partial class DashboardView : Window
    {
        private readonly DashboardViewModel _viewModel;

        // Primary ctor – IReportService injected by DI
        public DashboardView(IReportService reportService)
        {
            InitializeComponent();
            _viewModel = new DashboardViewModel(reportService);
            DataContext = _viewModel;
        }

        // Fallback for XAML designer – uses the static App.Services
        public DashboardView()
            : this(App.Services.GetRequiredService<IReportService>())
        {
        }

        private void OpenDialog(Window dialog, System.Windows.Controls.Button? sourceButton = null)
        {
            try
            {
                if (sourceButton != null) sourceButton.IsEnabled = false;
                dialog.Owner = this;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dialog.ShowInTaskbar = false;
                dialog.ShowDialog();
            }
            finally
            {
                if (sourceButton != null) sourceButton.IsEnabled = true;
                if (dialog.Owner == this) dialog.Owner = null;
            }
        }

        private void Products_Click(object sender, RoutedEventArgs e) =>
            OpenDialog(App.Services.GetRequiredService<ProductsView>(), sender as System.Windows.Controls.Button);

        private void POS_Click(object sender, RoutedEventArgs e) =>
            OpenDialog(App.Services.GetRequiredService<POSWindow>(), sender as System.Windows.Controls.Button);

        private void BarcodePrint_Click(object sender, RoutedEventArgs e) =>
            OpenDialog(App.Services.GetRequiredService<LabelGeneratorWindow>(), sender as System.Windows.Controls.Button);

        private void Reports_Click(object sender, RoutedEventArgs e) =>
            OpenDialog(App.Services.GetRequiredService<ReportsWindow>(), sender as System.Windows.Controls.Button);

        private void Settings_Click(object sender, RoutedEventArgs e) =>
            OpenDialog(App.Services.GetRequiredService<SettingsWindow>(), sender as System.Windows.Controls.Button);

        private void Exit_Click(object sender, RoutedEventArgs e) =>
            Close();
    }
}