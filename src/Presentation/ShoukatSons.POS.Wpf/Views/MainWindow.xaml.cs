using System.Windows;
using ShoukatSons.POS.Wpf.Services.Navigation;

namespace ShoukatSons.POS.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow(INavigationService navigationService)
        {
            InitializeComponent();
            DataContext = navigationService;
        }
    }
}