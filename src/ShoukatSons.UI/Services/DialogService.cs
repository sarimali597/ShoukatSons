// ─────────────────────────────────────────────────────
// File: Services/DialogService.cs
// ─────────────────────────────────────────────────────
using ShoukatSons.UI.Services.Interfaces;
using WpfMessageBox       = System.Windows.MessageBox;
using WpfMessageBoxButton = System.Windows.MessageBoxButton;
using WpfMessageBoxImage  = System.Windows.MessageBoxImage;
using WpfMessageBoxResult = System.Windows.MessageBoxResult;

namespace ShoukatSons.UI.Services
{
    public class DialogService : IDialogService
    {
        public void Info(
            string message,
            string title = "Info") =>
            WpfMessageBox.Show(
                message,
                title,
                WpfMessageBoxButton.OK,
                WpfMessageBoxImage.Information);

        public void Warn(
            string message,
            string title = "Warning") =>
            WpfMessageBox.Show(
                message,
                title,
                WpfMessageBoxButton.OK,
                WpfMessageBoxImage.Warning);

        public void Error(
            string message,
            string title = "Error") =>
            WpfMessageBox.Show(
                message,
                title,
                WpfMessageBoxButton.OK,
                WpfMessageBoxImage.Error);

        public bool Confirm(
            string message,
            string title = "Confirm",
            WpfMessageBoxButton buttons = WpfMessageBoxButton.YesNo) =>
            WpfMessageBox.Show(
                message,
                title,
                buttons,
                WpfMessageBoxImage.Question)
            == WpfMessageBoxResult.Yes;
    }
}