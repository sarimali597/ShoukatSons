using System.Windows; // WPF message boxes
using MessageBox = System.Windows.MessageBox; // 👈 explicitly alias WPF MessageBox

namespace ShoukatSons.UI.Services
{
    public class DialogService
    {
        public void Info(string message, string title = "Info") =>
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

        public void Error(string message, string title = "Error") =>
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

        public bool Confirm(string message, string title = "Confirm") =>
            MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }
}