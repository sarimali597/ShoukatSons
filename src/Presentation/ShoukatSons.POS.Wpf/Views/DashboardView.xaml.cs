using System.Windows.Controls;
using ShoukatSons.POS.Wpf.ViewModels;

namespace ShoukatSons.POS.Wpf.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView(DashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}