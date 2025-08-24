// ─────────────────────────────────────────────────────
// File: Views/POSWindow.xaml.cs
// ─────────────────────────────────────────────────────
using System.Windows;
using ShoukatSons.UI.ViewModels;

namespace ShoukatSons.UI.Views
{
    public partial class POSWindow : Window
    {
        public POSWindow(POSViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}