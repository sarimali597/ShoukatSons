using System.ComponentModel;

namespace ShoukatSons.UI.Services.Interfaces
{
    public interface INavigationService : INotifyPropertyChanged
    {
        object? Current { get; }
        void Navigate(object viewModel);
        object? GoBack();

        void Show<TWindow>() where TWindow : System.Windows.Window;
        bool? ShowDialog<TWindow>() where TWindow : System.Windows.Window;
        void CloseMainWindow();
    }
}