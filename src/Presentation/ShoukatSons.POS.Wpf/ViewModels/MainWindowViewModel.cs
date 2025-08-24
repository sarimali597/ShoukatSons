using System.Windows.Input;
using ShoukatSons.POS.Wpf.Utilities;                   // RelayCommand
using ShoukatSons.POS.Wpf.Services.Navigation;        // INavigationService
using ShoukatSons.POS.Wpf.ViewModels.Base;            // ViewModelBase
using ShoukatSons.POS.Wpf.ViewModels;                 // LoginViewModel, DashboardViewModel, StockAddViewModel, StockUpdateViewModel, SalesReportViewModel, CustomerViewModel
using ShoukatSons.POS.Wpf.ViewModels.Products;        // ProductsViewModel
using ShoukatSons.POS.Wpf.ViewModels.POS;             // POSViewModel
using ShoukatSons.POS.Wpf.ViewModels.BarcodePrint;    // BarcodePrintViewModel

namespace ShoukatSons.POS.Wpf.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase? _currentViewModel;
        public ViewModelBase? CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public ICommand NavigateCommand { get; }

        private readonly INavigationService _navigation;

        public MainWindowViewModel(INavigationService navigation)
        {
            _navigation      = navigation;
            CurrentViewModel = _navigation.CurrentViewModel;

            // wire up nav changes
            _navigation.CurrentViewModelChanged += () =>
                CurrentViewModel = _navigation.CurrentViewModel;

            // handle button clicks
            NavigateCommand = new RelayCommand(param => OnNavigate(param as string));
        }

        private void OnNavigate(string? screen)
        {
            if (string.IsNullOrEmpty(screen))
                return;

            switch (screen)
            {
                case "Login":
                    _navigation.NavigateTo<LoginViewModel>();
                    break;
                case "Dashboard":
                    _navigation.NavigateTo<DashboardViewModel>();
                    break;
                case "Products":
                    _navigation.NavigateTo<ProductsViewModel>();
                    break;
                case "POS":
                    _navigation.NavigateTo<POSViewModel>();
                    break;
                case "BarcodePrint":
                    _navigation.NavigateTo<BarcodePrintViewModel>();
                    break;
                case "StockAdd":
                    _navigation.NavigateTo<StockAddViewModel>();
                    break;
                case "StockUpdate":
                    _navigation.NavigateTo<StockUpdateViewModel>();
                    break;
                case "SalesReport":
                    _navigation.NavigateTo<SalesReportViewModel>();
                    break;
                case "Customer":
                    _navigation.NavigateTo<CustomerViewModel>();
                    break;
            }
        }
    }
}