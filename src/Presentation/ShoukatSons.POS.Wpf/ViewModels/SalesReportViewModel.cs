using System.Windows.Input;
using ShoukatSons.POS.Wpf.Services.Navigation;
using ShoukatSons.POS.Wpf.Utilities;
using ShoukatSons.POS.Wpf.ViewModels.Base;

namespace ShoukatSons.POS.Wpf.ViewModels
{
    public sealed class SalesReportViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;

        public SalesReportViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            ExitCommand = new RelayCommand(() => _navigation.NavigateTo<DashboardViewModel>());
        }

        public string Header => "Sales Report";
        public string Info   => "View daily, monthly, and yearly sales data.";

        public ICommand ExitCommand { get; }
    }
}