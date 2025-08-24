using System.Windows.Input;
using ShoukatSons.POS.Wpf.Services.Navigation;
using ShoukatSons.POS.Wpf.Utilities;
using ShoukatSons.POS.Wpf.ViewModels.Base;

namespace ShoukatSons.POS.Wpf.ViewModels
{
    public sealed class StockAddViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;

        public StockAddViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            ExitCommand = new RelayCommand(() => _navigation.NavigateTo<DashboardViewModel>());
        }

        public string Header => "Add Stock";
        public string Info   => "Register new inventory items here.";

        public ICommand ExitCommand { get; }
    }
}