using System.Windows.Input;
using ShoukatSons.POS.Wpf.Services.Navigation;
using ShoukatSons.POS.Wpf.Utilities;
using ShoukatSons.POS.Wpf.ViewModels;        // ← Added
using ShoukatSons.POS.Wpf.ViewModels.Base;

namespace ShoukatSons.POS.Wpf.ViewModels.POS
{
    public sealed class POSViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;

        public POSViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            ExitCommand = new RelayCommand(_ => _navigation.NavigateTo<DashboardViewModel>());
        }

        public string Header => "Point of Sale";
        public string Info   => "Process transactions.";
        public ICommand ExitCommand { get; }
    }
}