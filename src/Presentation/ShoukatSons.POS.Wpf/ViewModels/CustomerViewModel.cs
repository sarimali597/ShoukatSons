using System.Windows.Input;
using ShoukatSons.POS.Wpf.Services.Navigation;
using ShoukatSons.POS.Wpf.Utilities;
using ShoukatSons.POS.Wpf.ViewModels.Base;

namespace ShoukatSons.POS.Wpf.ViewModels
{
    public sealed class CustomerViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;

        public CustomerViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            ExitCommand = new RelayCommand(() => _navigation.NavigateTo<DashboardViewModel>());
        }

        public string Header => "Customer Management";
        public string Info   => "Add, update, or search customer records.";

        public ICommand ExitCommand { get; }
    }
}