using System;
using System.Windows.Input;
using ShoukatSons.POS.Wpf.Services.Navigation;
using ShoukatSons.POS.Wpf.ViewModels.Base;
using ShoukatSons.POS.Wpf.Utilities;               // RelayCommand
using ShoukatSons.POS.Wpf.ViewModels;              // DashboardViewModel

namespace ShoukatSons.POS.Wpf.ViewModels.Reports
{
    public class ReportsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigation;

        public string Header { get; }
        public string Info   { get; }

        public ICommand ExitCommand { get; }

        public ReportsViewModel(INavigationService navigation)
        {
            _navigation = navigation 
                ?? throw new ArgumentNullException(nameof(navigation));

            // Customize these as needed
            Header = "Sales & Inventory Reports";
            Info   = $"Generated on {DateTime.Now:dddd, MMMM d yyyy}";

            ExitCommand = new RelayCommand(_ => OnExit());
        }

        private void OnExit()
        {
            // Navigate back to Dashboard (or any other ViewModel)
            _navigation.NavigateTo<DashboardViewModel>();
        }
    }
}