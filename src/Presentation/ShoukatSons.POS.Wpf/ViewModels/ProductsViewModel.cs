using System.Windows.Input;
using ShoukatSons.POS.Wpf.Services.Navigation;
using ShoukatSons.POS.Wpf.Utilities;
using ShoukatSons.POS.Wpf.ViewModels;        // ← Added
using ShoukatSons.POS.Wpf.ViewModels.Base;

namespace ShoukatSons.POS.Wpf.ViewModels.Products
{
    public sealed class ProductsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;

        public ProductsViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            ExitCommand = new RelayCommand(_ => _navigation.NavigateTo<DashboardViewModel>());
        }

        public string Header => "Products Management";
        public string Info   => "Add, edit, and view products.";
        public ICommand ExitCommand { get; }
    }
}