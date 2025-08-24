// ─────────────────────────────────────────────────────
// File: Services/WindowService.cs
// ─────────────────────────────────────────────────────
using System.Linq;
using System.Windows;                          // for Window
using ShoukatSons.UI.Services.Interfaces;

using WpfApplication = System.Windows.Application;  // alias WPF’s Application

namespace ShoukatSons.UI.Services
{
    public class WindowService : IWindowService
    {
        public void CloseWindow(object viewModel)
        {
            // Find the open WPF Window with matching DataContext and close it
            var window = WpfApplication.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => ReferenceEquals(w.DataContext, viewModel));

            window?.Close();
        }
    }
}