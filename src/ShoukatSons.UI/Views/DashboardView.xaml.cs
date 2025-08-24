using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ShoukatSons.UI.ViewModels;

namespace ShoukatSons.UI.Views
{
    public partial class DashboardView : Window
    {
        public DashboardView(DashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        // Designer support
        public DashboardView() : this(App.Services.GetRequiredService<DashboardViewModel>()) { }
    }
}