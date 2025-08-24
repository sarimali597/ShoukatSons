using System;
using System.Windows.Input;
using ShoukatSons.POS.Wpf.Services.Navigation;
using ShoukatSons.POS.Wpf.Services.Session;
using ShoukatSons.POS.Wpf.Utilities;              // RelayCommand
using ShoukatSons.POS.Wpf.ViewModels.Base;       // ViewModelBase
using ShoukatSons.POS.Wpf.ViewModels.Products;   // ProductsViewModel
using ShoukatSons.POS.Wpf.ViewModels.POS;        // POSViewModel
using ShoukatSons.POS.Wpf.ViewModels.BarcodePrint; // BarcodePrintViewModel
using ShoukatSons.Services;                      // IPrintService

namespace ShoukatSons.POS.Wpf.ViewModels
{
    public sealed class DashboardViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;
        private readonly IPrintService      _printService;
        private readonly ISessionService    _session;

        public string ShopTitle => "Shoukat Sons Garments";
        public string Greeting
            => string.IsNullOrWhiteSpace(_session.Username)
               ? "Dashboard ready. Navigate to any section."
               : $"Welcome {_session.Username} ({_session.Role}).";

        public ICommand NavigateProductsCommand     { get; }
        public ICommand NavigatePOSCommand          { get; }
        public ICommand NavigateBarcodePrintCommand { get; }
        public ICommand TestPrintCommand            { get; }
        public ICommand LogoutCommand               { get; }

        public DashboardViewModel(
            INavigationService navigation,
            IPrintService printService,
            ISessionService session)
        {
            _navigation   = navigation;
            _printService = printService;
            _session      = session;

            NavigateProductsCommand     = new RelayCommand(_ => _navigation.NavigateTo<ProductsViewModel>());
            NavigatePOSCommand          = new RelayCommand(_ => _navigation.NavigateTo<POSViewModel>());
            NavigateBarcodePrintCommand = new RelayCommand(_ => _navigation.NavigateTo<BarcodePrintViewModel>());
            TestPrintCommand            = new RelayCommand(_ => TestPrint());
            LogoutCommand               = new RelayCommand(_ =>
            {
                _session.Clear();
                _navigation.NavigateTo<LoginViewModel>();
            });
        }

        private void TestPrint()
        {
            var sample =
                $"{ShopTitle}\n" +
                "---------------------\n" +
                "Software Test Print\n" +
                DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") +
                "\n---------------------\nThank you!";

            _printService.Configure("Your Printer Name", sample);
            _printService.Print();
        }
    }
}