using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

using ShoukatSons.POS.Wpf.Services.Navigation;
using ShoukatSons.POS.Wpf.Services.Session;
using ShoukatSons.POS.Wpf.Services.Print;
using ShoukatSons.POS.Wpf.Views.BarcodePrint;
using ShoukatSons.POS.Wpf.ViewModels;
using ShoukatSons.POS.Wpf.ViewModels.BarcodePrint;

namespace ShoukatSons.POS.Wpf
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow = mainWindow;
            mainWindow.Show();

            var nav = _serviceProvider.GetRequiredService<INavigationService>();
            nav.NavigateTo<DashboardViewModel>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // core services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISessionService, SessionService>();
            services.AddSingleton<IPrintService, PrintService>();

            // ViewModels
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<ProductsViewModel>();
            services.AddTransient<POSViewModel>();
            services.AddTransient<BarcodePrintViewModel>();
            services.AddTransient<LoginViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddSingleton<BarcodePrintView>();
        }
    }
}