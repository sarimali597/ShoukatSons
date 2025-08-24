using System.Windows.Input;
using ShoukatSons.POS.Wpf.Services.Navigation;
using ShoukatSons.POS.Wpf.Utilities;
using ShoukatSons.POS.Wpf.ViewModels.Base;

namespace ShoukatSons.POS.Wpf.ViewModels.BarcodePrint
{
    public sealed class BarcodePrintViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;

        public BarcodePrintViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            ExitCommand = new RelayCommand(_ => _navigation.NavigateTo<DashboardViewModel>());
        }

        public string Header => "Barcode Print";
        public string Info   => "Print item barcodes.";
        public ICommand ExitCommand { get; }
    }
}