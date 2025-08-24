using System.Windows.Input;
using ShoukatSons.POS.Wpf.Services.Navigation;
using ShoukatSons.POS.Wpf.Utilities;
using ShoukatSons.POS.Wpf.ViewModels.Base;

namespace ShoukatSons.POS.Wpf.ViewModels
{
    public sealed class StockUpdateViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;

        public StockUpdateViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            ExitCommand = new RelayCommand(() => _navigation.NavigateTo<DashboardViewModel>());
        }

        public string Header => "Update Stock";
        public string Info   => "Modify existing inventory details here.";

        public ICommand ExitCommand { get; }
    }
}