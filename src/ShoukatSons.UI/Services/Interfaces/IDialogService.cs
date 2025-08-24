using System.Windows;

namespace ShoukatSons.UI.Services.Interfaces
{
    public interface IDialogService
    {
        void Info(string message, string title = "Info");
        void Warn(string message, string title = "Warning");
        void Error(string message, string title = "Error");
        bool Confirm(string message, string title = "Confirm", MessageBoxButton buttons = MessageBoxButton.YesNo);
    }
}