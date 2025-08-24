using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoukatSons.Core.Models;
using ShoukatSons.Data;
using ShoukatSons.Services;
using ShoukatSons.Services.Interfaces;
using ShoukatSons.Services.Services;
using ShoukatSons.UI.Helpers;
using ShoukatSons.UI.Services;
using ShoukatSons.UI.Views;

namespace ShoukatSons.UI
{
    public partial class App : System.Windows.Application
    {
        public static SessionManager Sessions { get; } = new SessionManager();
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var sc = new ServiceCollection();

                // Ensure data folder exists
                var dataDir = Path.Combine(@"C:\ShoukatSons", "data");
                Directory.CreateDirectory(dataDir);

                // Single SQLite file for both contexts
                var dbPath = Path.Combine(dataDir, "shoukatsons.db");

                // Core application context
                sc.AddDbContext<AppDbContext>(opts =>
                    opts.UseSqlite($"Data Source={dbPath}"));

                // POS-specific context
                sc.AddDbContext<POSDbContext>(opts =>
                    opts.UseSqlite($"Data Source={dbPath}"));

                // Application services
                sc.AddSingleton(Sessions)
                  .AddTransient<IStockAlertService, StockAlertService>()
                  .AddHostedService<LowStockAlertService>()
                  .AddTransient<IReportService, ReportService>();

                // WPF views for DI
                sc.AddTransient<DashboardView>()
                  .AddTransient<ProductsView>()
                  .AddTransient<POSWindow>()
                  .AddTransient<LabelGeneratorWindow>()
                  .AddTransient<ReportsWindow>()
                  .AddTransient<SettingsWindow>();

                var provider = sc.BuildServiceProvider();

                // Apply pending EF Core migrations
                using (var scope = provider.CreateScope())
                {
                    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
                    scope.ServiceProvider.GetRequiredService<POSDbContext>().Database.Migrate();
                }

                Services = provider;

                // Shared WPF resources
                Resources["BoolToVisibilityConverter"] = new BoolToVisibilityConverter();

                // Hard-coded admin session for now
                Sessions.CurrentUser = new User
                {
                    Id = 1,
                    Username = "admin",
                    Role = UserRole.Admin
                };

                // Launch main window via DI
                var main = Services.GetRequiredService<DashboardView>();
                MainWindow = main;
                main.Show();
            }
            catch (Exception ex)
{
    System.Windows.MessageBox.Show(
        $"Startup error:\n{ex.Message}",
        "Application Error",
        MessageBoxButton.OK,
        MessageBoxImage.Error
    );
    Shutdown();
}
        }
    }
}